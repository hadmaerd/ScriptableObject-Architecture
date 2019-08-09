using UnityEngine;

public abstract class BaseVariable : GameEventBase
{
    public abstract bool ReadOnly { get; }
    public abstract bool PreserveInitValue { get; }
    public abstract System.Type Type { get; }
    public abstract object BaseValue { get; set; }
}
public abstract class BaseVariable<T> : BaseVariable
{
    public virtual T Value
    {
        get
        {
            return (_preserveInitValue && Application.isPlaying) ? _runtimeValue : _value;
        }
        set
        {
            if (_preserveInitValue && Application.isPlaying)
                _runtimeValue = SetValue (value);
            else
                _value = SetValue (value);
            Raise ();
        }
    }
    public override bool ReadOnly { get { return _readOnly; } }    
    public override bool PreserveInitValue { get { return _preserveInitValue; } }
    public override System.Type Type { get { return typeof(T); } }
    public override object BaseValue
    {
        get
        {
            return _value;
        }
        set
        {
            _value = SetValue((T)value);
            Raise();
        }
    }

    [SerializeField]
    protected T _value = default(T);
    [SerializeField]
    protected T _runtimeValue = default(T);
    [SerializeField]
    private bool _readOnly = false;
    [SerializeField]
    private bool _preserveInitValue = false;
    [SerializeField]
    private bool _raiseWarning = true;

    private void OnEnable ()
    {
        _runtimeValue = _value;
    }
    public virtual T SetValue(T value)
    {
        if (_readOnly)
        {
            RaiseReadonlyWarning();
            return Value;
        }

        return value;
    }
    public virtual T SetValue(BaseVariable<T> value)
    {
        if (_readOnly)
        {
            RaiseReadonlyWarning();
            return Value;
        }

        return value.Value;
    }
    private void RaiseReadonlyWarning()
    {
        if (!_readOnly || !_raiseWarning)
            return;

        Debug.LogWarning("Tried to set value on " + name + ", but value is readonly!", this);
    }
    
    public override string ToString()
    {
        return Value == null ? "null" : Value.ToString();
    }
    public static implicit operator T(BaseVariable<T> variable)
    {
        return variable.Value;
    }
}