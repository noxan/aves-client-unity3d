public class Tuple<A, B> {
  private A a;
  private B b;

  public Tuple(A a, B b) {
    this.a = a;
    this.b = b;
  }

  public A GetFirst() {
    return a;
  }
  public B GetSecond() {
    return b;
  }

  public void SetFirst(A a) {
    this.a = a;
  }
  public void SetSecond(B b) {
    this.b = b;
  }
}
