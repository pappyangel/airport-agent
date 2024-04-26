using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Port Authority of New York and New Jersey API", Description = "API that provides various information for the Port Authority of New York and New Jersey", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
//     { }

app.UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((swagger, httpReq) =>
        {
            swagger.Servers = new List<OpenApiServer>
            {
                    // new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" }
                    new OpenApiServer { Url = $"http://localhost:5178" }

            };
        });
    });
app.UseSwaggerUI();
app.UseStaticFiles();

//app.UseHttpsRedirection();

app.MapGet("/TSAWait/{airportCode}", (string airportCode) =>
{
    Random random = new();
    int waitTime = 0;
    string airportCodeUpper = airportCode.ToUpper();

    switch (airportCodeUpper)
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
.WithName("TSA Wait Time")
.WithOpenApi();

app.MapGet("/WalkTime/{airportCode}", (string airportCode) =>
{
    Random random = new();
    int walkTime = 0;
    string airportCodeUpper = airportCode.ToUpper();

    switch (airportCodeUpper)
    {
        case "EWR":
            walkTime = random.Next(3, 8);
            break;
        case "LGA":
            walkTime = random.Next(9, 12);
            break;
        case "JFK":
            walkTime = random.Next(13, 15);
            break;
        default:
            walkTime = 2;
            break;
    }

    double TimeOfDayFactor = CalcTimeOfDayFactor();
    walkTime = (int)(walkTime * TimeOfDayFactor);

    app.Logger.LogInformation("Walk Time endpoint called with airportCode: {airportCode} and walk time: {walkTime}", airportCode, walkTime);
    return new { WalkTime = walkTime };
    //return TypedResults.Ok(new { WaitTime = waitTime });

})
.WithDescription("Calculates the Average Walk Time to Gates from Terminal Security Checkpoint for an airport when provided a 3 digit code as a string")
//.WithDisplayName("Calculates the TSA wait time for a given airport code")
.WithName("Walk Time to Gates")
.WithOpenApi();

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