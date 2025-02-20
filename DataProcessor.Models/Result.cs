﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor.Models
{
	public class Result : IResult
	{
		public Guid Id { get; set; }
		public DateTime DateTime { get; set; }
		public string EntityId { get; set; }
		public int EntityTypeId { get; set; }
		public bool HasError { get; set; }
		public string ErrorMessage { get; set; }
	}
}
