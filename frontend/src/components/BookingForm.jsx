import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { CardElement, useStripe, useElements } from '@stripe/react-stripe-js';
import { getQuote, createBooking } from '../services/api';

function BookingForm({ startDate, endDate }) {
  const location = useLocation();
  const tentType = location.state?.tentType || '20x40 Event Tent'; // Default fallback
  const [customerName, setCustomerName] = useState('');
  const [customerEmail, setCustomerEmail] = useState('');
  const [address, setAddress] = useState('');
  const [city, setCity] = useState('');
  const [state, setState] = useState('');
  const [zipCode, setZipCode] = useState('');
  const [billingAddress, setBillingAddress] = useState('');
  const [billingCity, setBillingCity] = useState('');
  const [billingState, setBillingState] = useState('');
  const [billingZipCode, setBillingZipCode] = useState('');
  const [isBillingSameAsEvent, setIsBillingSameAsEvent] = useState(true);
  const [specialRequests, setSpecialRequests] = useState('');
  const [quote, setQuote] = useState(null);
  const [isFetchingQuote, setIsFetchingQuote] = useState(false);
  const [bookingError, setBookingError] = useState('');

  const stripe = useStripe();
  const elements = useElements();
  const navigate = useNavigate();

  useEffect(() => {
    const fetchQuote = async () => {
      if (startDate && endDate && address && city && state && zipCode) {
        setIsFetchingQuote(true);
        try {
          const quoteRequest = {
            startDate: startDate.toISOString(),
            endDate: endDate.toISOString(),
            address: { address, city, state, zipCode },
          };
          const quoteData = await getQuote(quoteRequest);
          setQuote(quoteData);
        } catch (error) {
          console.error('Failed to fetch quote:', error);
          setQuote(null);
        }
        setIsFetchingQuote(false);
      }
    };

    const debounce = setTimeout(() => {
        fetchQuote();
    }, 500);

    return () => clearTimeout(debounce);
  }, [startDate, endDate, address, city, state, zipCode]);

  const handleSubmit = async (event) => {
    event.preventDefault();
    if (!stripe || !elements || !quote) {
      return;
    }

    const cardElement = elements.getElement(CardElement);
    const { error, paymentMethod } = await stripe.createPaymentMethod({
      type: 'card',
      card: cardElement,
    });

    if (error) {
      console.log('[error]', error);
      setBookingError(error.message);
    } else {
      setBookingError(''); // Clear previous errors
      try {
        await createBooking({
          eventDate: startDate.toISOString(),
          eventEndDate: endDate.toISOString(),
          customerName,
          customerEmail,
          address,
          city,
          state,
          zipCode,
          billingAddress: isBillingSameAsEvent ? null : billingAddress,
          billingCity: isBillingSameAsEvent ? null : billingCity,
          billingState: isBillingSameAsEvent ? null : billingState,
          billingZipCode: isBillingSameAsEvent ? null : billingZipCode,
          specialRequests,
          paymentMethodId: paymentMethod.id,
          tentType: tentType,
        });
        navigate('/confirmation');
      } catch (err) {
        setBookingError(err.response?.data?.message || err.message || 'Booking failed.');
      }
    }
  };

  const cardElementOptions = {
    style: {
      base: {
        color: '#32325d',
        fontFamily: 'Arial, sans-serif',
        fontSmoothing: 'antialiased',
        fontSize: '16px',
        '::placeholder': {
          color: '#aab7c4'
        }
      },
      invalid: {
        color: '#fa755a',
        iconColor: '#fa755a'
      }
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <h2 className="text-lg font-semibold text-center text-gray-900 dark:text-white">
        Booking for: {startDate.toLocaleDateString()} to {endDate.toLocaleDateString()}
      </h2>
      
      <div>
        <label htmlFor="customerName" className="block text-sm font-bold text-gray-700 dark:text-gray-300 mb-1">Name</label>
        <input id="customerName" type="text" value={customerName} onChange={(e) => setCustomerName(e.target.value)} required className="p-2 rounded-md border border-gray-300 dark:border-gray-600 w-full bg-white dark:bg-gray-800 text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400" />
      </div>
      <div>
        <label htmlFor="customerEmail" className="block text-sm font-bold text-gray-700 dark:text-gray-300 mb-1">Email</label>
        <input id="customerEmail" type="email" value={customerEmail} onChange={(e) => setCustomerEmail(e.target.value)} required className="p-2 rounded-md border border-gray-300 dark:border-gray-600 w-full bg-white dark:bg-gray-800 text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400" />
      </div>
      <div>
        <label htmlFor="address" className="block text-sm font-bold text-gray-700 dark:text-gray-300 mb-1">Address</label>
        <input id="address" type="text" value={address} onChange={(e) => setAddress(e.target.value)} required className="p-2 rounded-md border border-gray-300 dark:border-gray-600 w-full bg-white dark:bg-gray-800 text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400" />
      </div>
      <div>
        <label htmlFor="city" className="block text-sm font-bold text-gray-700 dark:text-gray-300 mb-1">City</label>
        <input id="city" type="text" value={city} onChange={(e) => setCity(e.target.value)} required className="p-2 rounded-md border border-gray-300 dark:border-gray-600 w-full bg-white dark:bg-gray-800 text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400" />
      </div>
      <div>
        <label htmlFor="state" className="block text-sm font-bold text-gray-700 dark:text-gray-300 mb-1">State</label>
        <input id="state" type="text" value={state} onChange={(e) => setState(e.target.value)} required className="p-2 rounded-md border border-gray-300 dark:border-gray-600 w-full bg-white dark:bg-gray-800 text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400" />
      </div>
      <div>
        <label htmlFor="zipCode" className="block text-sm font-bold text-gray-700 dark:text-gray-300 mb-1">Zip Code</label>
        <input id="zipCode" type="text" value={zipCode} onChange={(e) => setZipCode(e.target.value)} required className="p-2 rounded-md border border-gray-300 dark:border-gray-600 w-full bg-white dark:bg-gray-800 text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400" />
      </div>

      <div>
        <label htmlFor="sameAsEvent" className="flex items-center text-gray-900 dark:text-white">
          <input
            id="sameAsEvent"
            type="checkbox"
            checked={isBillingSameAsEvent}
            onChange={(e) => setIsBillingSameAsEvent(e.target.checked)}
            className="mr-2"
          />
          Billing address is the same as event address
        </label>
      </div>

      {!isBillingSameAsEvent && (
        <>
          <div>
            <label htmlFor="billingAddress" className="block text-sm font-bold text-gray-700 dark:text-gray-300 mb-1">Billing Address</label>
            <input id="billingAddress" type="text" value={billingAddress} onChange={(e) => setBillingAddress(e.target.value)} required className="p-2 rounded-md border border-gray-300 dark:border-gray-600 w-full bg-white dark:bg-gray-800 text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400" />
          </div>
          <div>
            <label htmlFor="billingCity" className="block text-sm font-bold text-gray-700 dark:text-gray-300 mb-1">Billing City</label>
            <input id="billingCity" type="text" value={billingCity} onChange={(e) => setBillingCity(e.target.value)} required className="p-2 rounded-md border border-gray-300 dark:border-gray-600 w-full bg-white dark:bg-gray-800 text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400" />
          </div>
          <div>
            <label htmlFor="billingState" className="block text-sm font-bold text-gray-700 dark:text-gray-300 mb-1">Billing State</label>
            <input id="billingState" type="text" value={billingState} onChange={(e) => setBillingState(e.target.value)} required className="p-2 rounded-md border border-gray-300 dark:border-gray-600 w-full bg-white dark:bg-gray-800 text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400" />
          </div>
          <div>
            <label htmlFor="billingZipCode" className="block text-sm font-bold text-gray-700 dark:text-gray-300 mb-1">Billing Zip Code</label>
            <input id="billingZipCode" type="text" value={billingZipCode} onChange={(e) => setBillingZipCode(e.target.value)} required className="p-2 rounded-md border border-gray-300 dark:border-gray-600 w-full bg-white dark:bg-gray-800 text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400" />
          </div>
        </>
      )}

      <div className="bg-gray-50 dark:bg-gray-800 p-4 rounded-md border border-gray-200 dark:border-gray-600">
        <h3 className="text-lg font-bold mb-2 text-gray-900 dark:text-white">Price Quote</h3>
        {isFetchingQuote && <p className="text-gray-700 dark:text-gray-300">Calculating price...</p>}
        {quote && !isFetchingQuote && (
            <>
                <p className="text-gray-900 dark:text-white">Rental Fee ({quote.rentalDays} days): ${quote.rentalFee.toFixed(2)}</p>
                <p className="text-gray-900 dark:text-white">Delivery Fee: ${quote.deliveryFee.toFixed(2)}</p>
                <p className="text-gray-900 dark:text-white">Refundable Security Deposit: ${quote.securityDeposit.toFixed(2)}</p>
                <p className="font-bold mt-2 text-gray-900 dark:text-white">Total Price: ${quote.totalPrice.toFixed(2)}</p>
            </>
        )}
        {!quote && !isFetchingQuote && <p className="text-gray-700 dark:text-gray-300">Please complete the address fields to calculate the price.</p>}
      </div>

      <div>
        <label htmlFor="specialRequests" className="block text-sm font-bold text-gray-700 dark:text-gray-300 mb-1">Special Requests</label>
        <textarea id="specialRequests" value={specialRequests} onChange={(e) => setSpecialRequests(e.target.value)} rows="3" className="p-2 rounded-md border border-gray-300 dark:border-gray-600 w-full bg-white dark:bg-gray-800 text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400" />
      </div>

      <div>
        <label htmlFor="card-element" className="block text-sm font-bold text-gray-700 dark:text-gray-300 mb-1">Credit or debit card</label>
        <div className="p-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-800">
          <CardElement id="card-element" options={cardElementOptions} />
        </div>
      </div>

      {bookingError && (
        <div className="text-red-700 dark:text-red-400 text-center p-2 bg-red-100 dark:bg-red-900/30 rounded border border-red-300 dark:border-red-700">
          {bookingError}
        </div>
      )}

      <button type="submit" disabled={!stripe || !quote || isFetchingQuote} className="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded disabled:bg-gray-400 dark:disabled:bg-gray-600">
        Book Now
      </button>
    </form>
  );
}

export default BookingForm;