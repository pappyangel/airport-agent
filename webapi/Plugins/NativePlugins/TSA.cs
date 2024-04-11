using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace CopilotChat.WebApi.Plugins.NativePlugins;
#pragma warning disable IDE0055 // Surpress formatting warning
public class TSAPlugin
{
  [KernelFunction, Description("Returns the TSA wait time for a specific airport code")]
  public int WaitTime([Description("The airport code")] string airportCode)
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
    return waitTime;
  }
}
