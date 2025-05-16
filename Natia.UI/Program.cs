using Microsoft.EntityFrameworkCore;
using Natia.Application.Contracts;
using Natia.Application.Services;
using Natia.Common.Services;
using Natia.Core.Context;
using Natia.Neurall.Interfaces;
using Natia.Neurall.Services;
using Natia.UI.Jobs;
using NatiaGuard.BrainStorm.Main;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCommonServices();
builder.Services.AddScoped<ISolutionRecommendationService, SolutionRecommendationService>();
builder.Services.AddScoped<INeuralMLPredict, NeuralMLPredict>();
builder.Services.AddScoped<IImapServices, ImapService>();
builder.Services.AddDbContext<SpeakerDbContext>(io =>
{

    io.UseSqlServer(builder.Configuration.GetConnectionString("Server=192.168.1.102;Database=JandagBase;User Id=Guga13guga;Password=Guga13gagno!;Encrypt=True;TrustServerCertificate=True;"));
});
builder.Services.AddScoped<Main>();
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
