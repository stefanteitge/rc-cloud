using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentResults;

namespace RcCloud.DateScraper.Application.Dmc.Calendar.Domain;
internal class ScrapeError(string message) : Error(message)
{
}
