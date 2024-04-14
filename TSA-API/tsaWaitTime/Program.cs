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

app.MapGet("/.well-known/ai-plugin.json", () =>
{
  var mimeType = "application/json"; // Set the appropriate MIME type for your file
  var path = @".well-known/ai-plugin.json"; // Specify the path to your file

  var pluginFile = File.ReadAllText(path);
  return pluginFile;
  //return Results.File(path, contentType: mimeType);

  // if (File.Exists(path))
  // {
  //   //return "The file does exist.";
  //   return Results.File(path, contentType: mimeType);
  // }
  // else
  // {
  //   return "The file does not exist.";
  // }
  //return "Hello Well Known";
  //TSA-API/tsaWaitTime/.well-known/ai-plugin.json

}).WithOpenApi();


app.MapGet("/hello", () => { return "Hello, World!"; });




app.Run();





