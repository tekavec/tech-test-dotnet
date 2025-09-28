namespace ClearBank.DeveloperTest.Types
{
    public class PaymentContext
    {
        public Account Account { get; private set; }
        public MakePaymentRequest MakePaymentRequest { get; private set; }

        public PaymentContext(Account account, MakePaymentRequest makePaymentRequest)
        {
            Account = account;
            MakePaymentRequest = makePaymentRequest;
        }
    }
}
