using ChessCore;

namespace ChessCoreTest;

public class BitVector8Tests
{
    private BitVector8 Target { get; set; } = null!;

    [SetUp]
    public void SetUp() => Target = new BitVector8();

    [Test]
    public void Index_must_be_between_0_and_7_inclusive()
    {
        Assert.Throws<IndexOutOfRangeException>(() =>
        {
            var _ = Target[-1];
        });

        Assert.Throws<IndexOutOfRangeException>(() =>
        {
            var _ = Target[8];
        });

        Assert.Throws<IndexOutOfRangeException>(() => { Target[-1] = false; });
        Assert.Throws<IndexOutOfRangeException>(() => { Target[8] = false; });
    }

    [Test]
    public void All_bits_are_initialised_false()
    {
        for (var i = 0; i < 8; i++)
            Assert.That(Target[i], Is.False);

        Assert.That(Target.ToString(), Is.EqualTo("00000000"));
    }

    [Test]
    public void All_bits_can_be_set_true()
    {
        for (var i = 0; i < 8; i++)
        {
            Target[i] = true;
            Assert.That(Target[i], Is.True);
        }

        Assert.That(Target.ToString(), Is.EqualTo("11111111"));
    }

    [Test]
    public void Bits_can_be_set_and_cleared_individually()
    {
        Target[3] = !Target[3];
        Assert.That(Target[3], Is.True);

        Target[3] = !Target[3];
        Assert.That(Target[3], Is.False);

        Target[4] = false;
        Assert.That(Target[4], Is.False);

        Target[4] = true;
        Target[4] = true;
        Assert.That(Target[4], Is.True);
    }
}