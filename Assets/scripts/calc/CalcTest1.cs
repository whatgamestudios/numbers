using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CalcTest1
{
    // A Test behaves as an ordinary method
    [Test]
    public void One() {
        shouldWork("1", 1);
    }    
    [Test]
    public void Two() {
        shouldWork("2", 2);
    }    
    [Test]
    public void Three() {
        shouldWork("3", 3);
    }    
    [Test]
    public void Four() {
        shouldWork("4", 4);
    }    
    [Test]
    public void Five() {
        shouldWork("5", 5);
    }    
    [Test]
    public void Six() {
        shouldWork("6", 6);
    }    
    [Test]
    public void Seven() {
        shouldWork("7", 7);
    }    
    [Test]
    public void Eight() {
        shouldWork("8", 8);
    }    
    [Test]
    public void Nine() {
        shouldWork("9", 9);
    }    
    [Test]
    public void Ten() {
        shouldWork("10", 10);
    }    
    [Test]
    public void TwentyFive() {
        shouldWork("25", 25);
    }    
    [Test]
    public void Fifty() {
        shouldWork("50", 50);
    }    
    [Test]
    public void OneHundred() {
        shouldWork("100", 100);
    }    

    [Test]
    public void OnePlus2() {
        shouldWork("1+2", 3);
    }    
    [Test]
    public void SevenMinusFive() {
        shouldWork("7-5", 2);
    }    
    [Test]
    public void SevenMultiplyFive() {
        shouldWork("7*5", 35);
    }    
    [Test]
    public void OneHundredDivideFive() {
        shouldWork("100/5", 20);
    }    


    [Test]
    public void OnePlus2Plus3() {
        shouldWork("1+2+3", 6);
    }    
    [Test]
    public void OnePlus2Plus3Plus4() {
        shouldWork("1+2+3+4", 10);
    }    

    [Test]
    public void OnePlus2Plus3Plus4Plus5() {
        shouldWork("1+2+3+4+5", 15);
    }

    [Test]
    public void OnePlus2Minus3Plus4Plus5() {
        shouldWork("1+2-3+4+5", 9);
    }

    [Test]
    public void Brackets1() {
        shouldWork("(1)", 1);
    }
    [Test]
    public void Brackets2() {
        shouldWork("(1+3)/2", 2);
    }

    private void shouldWork(string equation, int result) {
        CalcProcessor calc = new CalcProcessor();
        int res;
        int err;
        (res, err) = calc.calc(equation);
        Assert.Equals(res, result);
        Assert.Equals(err, CalcProcessor.ERR_NO_ERROR);
    }    

}
