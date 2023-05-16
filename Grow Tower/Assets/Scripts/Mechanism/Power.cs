public class Power
{
    private float _power;
    public float Strength 
    { 
        get { return _power; } 
        set
        {
            _power = value;
            OnVariableChange?.Invoke();
        } 
    }
    public string Strength_str
    {
        get
        {
            string construct = "";
            float tmp = 0f;
            if (_power / 1000000f >= 1)
            {
                tmp = _power / 1000000f;
                construct = tmp.ToString("0.##")+"M";
            }
            else if (_power / 1000f >= 1)
            {
                tmp = _power / 1000f;
                construct = tmp.ToString("0.##")+"K";                
            }
            else construct = _power.ToString();
            return construct;
        }
    }
    public Power(float Strength) => this.Strength = Strength; //if already initialized, this would do
    public Power() { } //this is used for initialized new object, so that at least the delegate works when subscribed

    public event OnVariableChangeDeletegate OnVariableChange;
    public delegate void OnVariableChangeDeletegate();
}
