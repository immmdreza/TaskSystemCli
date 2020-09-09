namespace TaskSystemCli
{
    public class Response
    {
        public string[] Triggers { get; set; }

        public string[] Answer { get; set; }

        public TriggerType TriggerType { get; set; }
    }

    public enum TriggerType
    {
        StartWith,
        EndWith,
        Partlcal,
        ExactMatch
    }
}
