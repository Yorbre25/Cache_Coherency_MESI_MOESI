namespace Proyecto_Arqui.Classes
{
    public class Transaction_tracker
    {
        public List<int> cache_resp { get; set; }
        public List<transaction> transactionList { get; set; }
        public int cost { get; set; }

        public IDictionary<string, int> com_types;
        public Transaction_tracker()
        {
            transactionList = new List<transaction>();
            cost = 0;
            com_types = new Dictionary<string, int>();
            com_types.Add("cache", 0);
            com_types.Add("pe", 0);
            com_types.Add("memory", 0);
        }
        public void add(Transaction_tracker tracker)
        {
            cache_resp = tracker.cache_resp;
            transactionList.AddRange(tracker.transactionList);
            cost += tracker.cost;
            if (tracker.com_types.ContainsKey("cache"))
            {
                com_types["cache"] += tracker.com_types["cache"];
            }
            if (tracker.com_types.ContainsKey("pe"))
            {
                com_types["pe"] += tracker.com_types["pe"];
            }
            if (tracker.com_types.ContainsKey("memory"))
            {
                com_types["memory"] += tracker.com_types["memory"];
            }
        }

        public void add(IDictionary<string, int> dict)
        {
            if (dict.ContainsKey("cache"))
            {
                this.com_types["cache"] += dict["cache"];
            }

            if (dict.ContainsKey("pe"))
            {
                this.com_types["pe"] += dict["pe"];
            }

            if (dict.ContainsKey("memory"))
            {
                this.com_types["memory"] += dict["memory"];
            }
        }
        public void updateCost()
        {
            cost = 0;
            if (com_types.ContainsKey("cache"))
            {
                cost += com_types["cache"] * 1;
            }
            if (com_types.ContainsKey("pe"))
            {
                cost += com_types["pe"] * 3;
            }
            if (com_types.ContainsKey("memory"))
            {
                cost += com_types["memory"] * 243;
            }
        }
    }
}
