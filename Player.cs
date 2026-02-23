class Player
{
    public int x { get; private set; }
    public int y { get; private set; }

    public string emotion { get; private set; }

    public Player() : this("😐") { }
    public Player(string emotion)
    {
        this.emotion = emotion;
        //PLAYER STARTS AT 2,2
        x = 2;
        y = 2;
    }
    public string DisplayEmotionalState()
    {
        return this.emotion;
    }
}
