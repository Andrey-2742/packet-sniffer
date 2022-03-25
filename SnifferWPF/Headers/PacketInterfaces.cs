namespace SnifferWPF.Headers
{
    public interface ITransportLevelHeader
    {
        public ushort Length { get; }
        public string Data { get; }
    }

    public interface IHasPortInformation
    {
        public ushort SourcePort { get; }
        public ushort DestinationPort { get; }
    }
}
