using CarBooking.Api;
using Microsoft.EntityFrameworkCore; // Ensure this namespace is included

SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BookingDbContext>(options =>
   options.UseSqlite("Data Source=car_rental.db"));

builder.Services.AddScoped<IBookingRepo, BookingRepo>();

builder.Services.Configure<PriceSettings>(
    builder.Configuration.GetSection("PriceSettings")
);

builder.Services.AddScoped<IBookingService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var dayPrice = config.GetValue<decimal>("PriceSettings:BaseDayRental");
    var kmPrice = config.GetValue<decimal>("PriceSettings:BaseKmPrice");
    var repo = sp.GetRequiredService<IBookingRepo>();
    return new BookingService(repo, dayPrice, kmPrice);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("https://localhost:3000")
               .AllowAnyHeader();
    });;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowFrontend");
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
    db.Database.EnsureCreated();
}

app.MapPost("/pickup", (Booking booking, IBookingService bookingService) =>
{
    bookingService.RegisterPickup(booking);
    return Results.Created($"/bookings/{booking.BookingNumber}", booking);
})
.WithName("Car Pickup")
.WithOpenApi();

app.MapGet("/bookings", async (IBookingService bookingService, int pageSize = 10, int pageNumber = 1) =>
{
    var bookings = await bookingService.GetBookings(pageSize, pageNumber);
    return Results.Ok(bookings);
});

app.MapPost("/return", async (int bookingNumber, DateTime returnDate, int returnKm, IBookingService bookingService) =>
{
    var cost = await bookingService.RegisterReturn(bookingNumber, returnDate, returnKm);
    return Results.Ok(cost);
});

app.UseHttpsRedirection();

app.Run();