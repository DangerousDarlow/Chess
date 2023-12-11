namespace ChessCore;

public class BitVector8
{
    private byte _bits;

    public bool this[int index]
    {
        get
        {
            if (index is < 0 or > 7)
                throw new IndexOutOfRangeException();

            return (_bits & (1 << index)) != 0;
        }

        set
        {
            if (index is < 0 or > 7)
                throw new IndexOutOfRangeException();

            if (value)
                _bits |= (byte) (1 << index);
            else
                _bits &= (byte) ~(1 << index);
        }
    }

    public override string ToString() => Convert.ToString(_bits, 2).PadLeft(8, '0');
}