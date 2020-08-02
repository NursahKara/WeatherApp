using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace IFS_Weather.Models
{
	[DataContract]
	public class DataPoint
	{
		public DataPoint(DateTime? x, int? y)
		{
			this.X = x;
			this.Y = y;
		}

		[DataMember(Name = "x")]
		public DateTime? X = null;

		[DataMember(Name = "y")]
		public int? Y = null;
	}
}