
namespace TeslasuitAlyx
{
    public struct HLAlyxFeedbackEventArgs
    {
        public HLAlyxFeedbackType FeedbackType { get; }
        public float Angle { get; }
        public float Height { get; }
        public float Multiplier { get; }
        public bool DontReplay { get; }

        public HLAlyxFeedbackEventArgs(HLAlyxFeedbackType feedbackType)
        {
            FeedbackType = feedbackType;
            Angle = 0;
            Height = 0;
            Multiplier = 0;
            DontReplay = false;
        }

        public HLAlyxFeedbackEventArgs(HLAlyxFeedbackType feedbackType, float angle, float height, float multiplier, bool dontReplay)
        {
            FeedbackType = feedbackType;
            Angle = angle;
            Height = height;
            Multiplier = multiplier;
            DontReplay = dontReplay;
        }

        public override string ToString()
        {
            return $"Type: {FeedbackType}, Angle: {Angle}, Height: {Height}, Multiplier: {Multiplier}, Overwrite: {DontReplay}";
        }
    }
}
