#pragma warning disable IDE0055 // Surpress formatting warning

using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new() { Title = "TSA Wait Time API", Description = "API that provides TSA wait times at various airports", Version = "v1" });
});
var app = builder.Build();
//app.UseSwagger();
app.UseSwagger(c =>
{
  c.PreSerializeFilters.Add((swagger, httpReq) =>
  {
    swagger.Servers = new List<OpenApiServer>
      {
            // new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" }
            new OpenApiServer { Url = $"http://localhost:5088" }

      };
  });
});
app.UseSwaggerUI();
app.UseStaticFiles();


app.MapGet("/TSAWait/{airportCode}", (string airportCode) =>
{ 
  Random random = new();
  int waitTime = 0;

  switch (airportCode)
  {
    case "EWR":
      waitTime = random.Next(10, 16);
      break;
    case "LGA":
      waitTime = random.Next(20, 30);
      break;
    case "JFK":
      waitTime = random.Next(35, 46);
      break;
    default:
      waitTime = 2;
      break;
  }

  double TimeOfDayFactor = CalcTimeOfDayFactor();
  waitTime = (int)(waitTime * TimeOfDayFactor);

  app.Logger.LogInformation("TSAWait endpoint called with airportCode: {airportCode} and wait time: {waitTime}", airportCode, waitTime);
  return new { WaitTime = waitTime };
  //return TypedResults.Ok(new { WaitTime = waitTime });

})
.WithDescription("Calculates the TSA wait time for an airport when provided a 3 digit code as a string")
//.WithDisplayName("Calculates the TSA wait time for a given airport code")
.WithName("TSA Wait Time API")
.WithOpenApi();

//app.MapGet("/hello", () => { return "Hello from TSA Wait Time!"; });


app.Run();

static double CalcTimeOfDayFactor()
{
  DateTime now = DateTime.Now;  
  TimeSpan factorEarlyMorning = new(6, 0, 0); // 6 AM
  TimeSpan factorLateMorning = new(10, 0, 0); // 10 AM
  TimeSpan factorAfternoon = new(15, 0, 0); // 3 PM
  TimeSpan factorLateNight = new(22, 0, 0); // 10 PM  
  TimeSpan nowTime = now.TimeOfDay;
  double TimeOfDayFactor = 1.0;

  if ((nowTime > factorEarlyMorning) && (nowTime < factorLateMorning))
  {
    // morning rush
    TimeOfDayFactor = 1.8;
  }
  else if ((nowTime > factorLateMorning) && (nowTime < factorAfternoon))
  {
    // mid day slow down
    TimeOfDayFactor = 1.2;
  }
  else if ((nowTime > factorAfternoon) && (nowTime < factorLateNight))
  {
    // end of day rush
    TimeOfDayFactor = 1.5;
  }
  else
  {
    // overnight slow down
    TimeOfDayFactor = .8;
  }

  return TimeOfDayFactor;
}