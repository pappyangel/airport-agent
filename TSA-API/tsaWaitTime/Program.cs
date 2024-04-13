//using Microsoft.AspNetCore.OpenApi;

#pragma warning disable IDE0055 // Surpress formatting warning

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();


app.MapGet("/TSAWait/{airportCode}", (string airportCode) =>
{

  int waitTime = 0;
  switch (airportCode)
  {
    case "EWR":
      waitTime = 15;
      break;
    case "LGA":
      waitTime = 30;
      break;
    case "JFK":
      waitTime = 45;
      break;
    default:
      waitTime = 10;
      break;
  }

  return new { WaitTime = waitTime };
  //return TypedResults.Ok(new { WaitTime = waitTime });

}).WithOpenApi();

app.Run();





