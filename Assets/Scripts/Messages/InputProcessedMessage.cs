public class InputProcessedMessage
{
    public InputRecord InputRecord { get; set; }

    public InputProcessedMessage(InputRecord inputRecord)
    {
        InputRecord = inputRecord;
    }
}