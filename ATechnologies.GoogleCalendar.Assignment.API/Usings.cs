global using System.ComponentModel.DataAnnotations;
global using System.Net;
global using System.Text;

global using ATechnologies.GoogleCalendar.Assignment.API.Dtos.AuthDtos;
global using ATechnologies.GoogleCalendar.Assignment.API.Dtos.CalendarDtos;
global using ATechnologies.GoogleCalendar.Assignment.API.Helpers;
global using ATechnologies.GoogleCalendar.Assignment.API.IServices;
global using ATechnologies.GoogleCalendar.Assignment.API.Services;
global using ATechnologies.GoogleCalendar.Assignment.API.Settings;

global using Google;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Options;

global using Newtonsoft.Json;
global using Newtonsoft.Json.Serialization;

global using RestSharp;
namespace ATechnologies.GoogleCalendar.Assignment.API;