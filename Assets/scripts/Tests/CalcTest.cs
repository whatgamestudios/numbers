using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CalcTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void CalcTestSimplePasses()
    {
        // CalcProcessor calc = new CalcProcessor();
        // int res;
        // int err;
        // (res, err) = calc.calc("(1)");
        // Assert.equals(res, 1);
        // Assert.equals(err, CalcProcessor.ERR_NO_ERROR);

        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator CalcTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
