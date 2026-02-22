class Player
{
    public int x { get; private set; }
    public int y { get; private set; }

    public string emotion { get; private set; }

    public Player() : this("😐") { }
    public Player(string emotion)
    {
        this.emotion = emotion;
    }
    public string DisplayEmotionalState()
    {
        return this.emotion;
    }
}
