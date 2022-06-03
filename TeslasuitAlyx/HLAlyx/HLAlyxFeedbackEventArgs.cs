
namespace TeslasuitAlyx
{
    public struct HLAlyxFeedbackEventArgs
    {
        public HLAlyxFeedbackType FeedbackType { get; }
        public float Angle { get; }
        public float Height { get; }
        public float Multiplier { get; }
        public bool Overwrite { get; }

        public HLAlyxFeedbackEventArgs(HLAlyxFeedbackType feedbackType)
        {
            FeedbackType = feedbackType;
            Angle = 0;
            Height = 0;
            Multiplier = 0;
            Overwrite = false;
        }

        public HLAlyxFeedbackEventArgs(HLAlyxFeedbackType feedbackType, float angle, float height, float multiplier, bool overwrite)
        {
            FeedbackType = feedbackType;
            Angle = angle;
            Height = height;
            Multiplier = multiplier;
            Overwrite = overwrite;
        }

        public override string ToString()
        {
            return $"Type: {FeedbackType}, Angle: {Angle}, Height: {Height}, Multiplier: {Multiplier}, Overwrite: {Overwrite}";
        }
    }
}
