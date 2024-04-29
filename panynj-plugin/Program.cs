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
    int waitTime = TSAWait(airportCode);

    app.Logger.LogInformation("TSAWait endpoint called with airportCode: {airportCode} and wait time: {waitTime}", airportCode, waitTime);

    return new { WaitTime = waitTime };    

})
.WithDescription("Calculates the TSA wait time for an airport when provided a 3 digit code as a string")
.WithName("TSA Wait Time")
.WithOpenApi();

app.MapGet("/WalkTime/{airportCode}", (string airportCode) =>
{
    int walkTime = WalkTime(airportCode);
        
    app.Logger.LogInformation("Walk Time endpoint called with airportCode: {airportCode} and walk time: {walkTime}", airportCode, walkTime);
    
    return new { WalkTime = walkTime };    

})
.WithDescription("Calculates the Average Walk Time to Gates from Terminal Security Checkpoint for an airport when provided a 3 digit code as a string")
.WithName("Walk Time to Gates")
.WithOpenApi();


app.MapGet("/FlightStatus/{airline}/{flightNumber}", (string airline, string flightNumber) =>
{   
    string flightStatus = FlightStatus(airline, flightNumber);
    app.Logger.LogInformation("Flight Status: {airline}, {flightNumber}, {flightStatus}", airline, flightNumber, flightStatus);
    return new { FlightStatus = flightStatus };   

})
.WithDescription("Returns the status for a flight when provided the airline and flight number")
.WithName("Flight Status")
.WithOpenApi();

// app.MapGet("/EstimatedArrivalTime/{airline}/{flightNumber}", (string airline, string flightNumber) =>
// {   
//     string estArrivalTime = "ETA not set";
//     estArrivalTime = TravelerTimeToAirport(airline, flightNumber);
    
//     app.Logger.LogInformation("Estimated Arrival Time: {airline}, {flightNumber}, {EstimatedArrivalTime}", airline, flightNumber, estArrivalTime);
//     return new { EstimatedArrivalTime = estArrivalTime };   

// })
// .WithDescription("Returns the Estimated Arrival Time for a traveler based on flight departure, TSA wait time and walk time to gate")
// .WithName("EstimatedArrivalTime")
// .WithOpenApi();

app.Run();

int TSAWait(string airportCode)
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

    return waitTime;
}

int WalkTime(string airportCode)
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

    return walkTime;
}

string TravelerTimeToAirport(string airline, string flightNumber)
{
    string airlineUpper = airline.ToUpper();
    string flightNumberUpper = flightNumber.ToUpper();
    string estArrivalTime = "ETA not set";
    string airportCode = "";
    int tsaWaitTime = 0;
    int walkTime = 0;
    DateTime ScheduledDepartureTime = DateTime.Now;
    DateTime EstimatedTravelerTimeToAirport = DateTime.Now;

    switch (airlineUpper)
    {
        case "JETBLUE":
            if (flightNumberUpper == "61613")
            {
                //JetBlue 61613, New York, JFK to Los Angeles, LAX, Scheduled departure 9:27 am
                ScheduledDepartureTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 27, 0);                
                airportCode = "JFK";
                tsaWaitTime = TSAWait(airportCode);
                walkTime = WalkTime(airportCode);
                
                EstimatedTravelerTimeToAirport = ScheduledDepartureTime.AddMinutes(-(tsaWaitTime + walkTime));
                estArrivalTime = EstimatedTravelerTimeToAirport.ToString("hh:mm tt");

            }
            break;        
        default:
            estArrivalTime = "Flight not found";
            break;
    }   

    app.Logger.LogInformation("TravelerTimeToAirport: {airportCode}, {tsaWaitTime}, {walkTime},{ScheduledDepartureTime},{EstimatedTravelerTimeToAirport}", airportCode, tsaWaitTime, walkTime,ScheduledDepartureTime,EstimatedTravelerTimeToAirport);

    return estArrivalTime;
}

string FlightStatus(string airline, string flightNumber)
{
    string airlineUpper = airline.ToUpper();
    string flightNumberUpper = flightNumber.ToUpper();
    string flightStatus = "Flight status not set";

    switch (airlineUpper)
    {
        case "JETBLUE":
            if (flightNumberUpper == "61613")
            {
                flightStatus = "JetBlue 61613, New York, JFK to Los Angeles, LAX, Scheduled departure 9:30 am, Scheduled arrival 12:37 pm";
            }
            break;        
        default:
            flightStatus = "Flight not found";
            break;
    }   

    return flightStatus;
}

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

