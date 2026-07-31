//public static string ToPersian(this int input)
//		{
//			return input.ToString().ToPersian();
//		}

//		public static string ToPersian(this double? input)
//		{
//			return input.ToString().ToPersian();
//		}

//		public static string ToPersian(this double input)
//		{
//			return input.ToString().ToPersian();
//		}

//		public static string ToPersian(this int? input)
//		{
//			return input == null ? 0.ToPersian() : input.Value.ToPersian();
//		}



//		

//		public static string ToPersianTime(this DateTime datetime)
//		{
//			Dictionary<string, string> numbers = new Dictionary<string, string>();
//			numbers.Add("1", "۱");
//			numbers.Add("2", "۲");
//			numbers.Add("3", "۳");
//			numbers.Add("4", "۴");
//			numbers.Add("5", "۵");
//			numbers.Add("6", "۶");
//			numbers.Add("7", "۷");
//			numbers.Add("8", "۸");
//			numbers.Add("9", "۹");
//			numbers.Add("0", "۰");

//			string[] timeSplited = datetime.ToShortTimeString().Split(':');

//			string hour = string.Empty;
//			foreach (char hourChar in timeSplited[0])
//			{
//				if (char.IsDigit(hourChar))
//					hour += numbers[hourChar.ToString()];
//			}
//			string minute = string.Empty;
//			foreach (char minuteChar in timeSplited[1])
//			{
//				if (char.IsDigit(minuteChar))
//					minute += numbers[minuteChar.ToString()];
//			}
//			string persianTime = hour + ":" + minute + (datetime.ToShortTimeString().Contains("AM") ? "قبل از ظهر" : "بعد از ظهر");
//			return persianTime;
//		}





