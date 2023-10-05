
namespace Spark.Class
{
	public class SparkInfo
	{
		public Guid ID { get; } = Guid.NewGuid();
        public string TimeStamp { get; } = DateTime.Now.ToString("dddd, dd MMMM yyyy");

        public int HT { get; set; }
		public int NT { get; set; }

		public SparkInfo(int ht, int nt)
		{
			this.HT = ht;
			this.NT = nt;
		}
	}
}

