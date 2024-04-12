using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

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

  // return new { WaitTime = 33 };
  return TypedResults.Ok(new { WaitTime = waitTime });

});





