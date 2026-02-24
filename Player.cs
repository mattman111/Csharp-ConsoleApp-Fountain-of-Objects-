class Player
{
    public int x { get; set; }
    public int y { get; set; }

    public string emotion { get; private set; }

    public Player() : this("😐") { }
    public Player(string emotion)
    {
        this.emotion = emotion;
        //PLAYER STARTS AT 2,2
        x = 2;
        y = 2;
    }
}
