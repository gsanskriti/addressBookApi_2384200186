using BusinessLayer.Interface;
using BusinessLayer.mapping;
using BusinessLayer.Service;
using BusinessLayer.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using ModelLayer.model;
using RepositoryLayer.Context;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Register DbContext with SQL Server 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Server=localhost;Database=AddressBook;User Id=sa;Password=Sanskiriti_3009;TrustServerCertificate=True;"));

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(AddressBookProfile));

// Register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<RequestModelValidator>();
builder.Services.AddFluentValidationAutoValidation();

// Register Business & Repository Layer Services
builder.Services.AddScoped<IAddressBL, AddressBL>();
builder.Services.AddScoped<IAddressRL, AddressRL>();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
