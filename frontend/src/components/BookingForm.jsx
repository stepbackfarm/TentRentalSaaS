import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { CardElement, useStripe, useElements } from '@stripe/react-stripe-js';
import { getQuote, createBooking } from '../services/api';

function BookingForm({ startDate, endDate }) {
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

  const stripe = useStripe();
  const elements = useElements();
  const navigate = useNavigate();

  useEffect(() => {
    const fetchQuote = async () => {
      if (startDate && endDate && address && city && state && zipCode) {
        setIsFetchingQuote(true);
        try {
          const quoteRequest = {
            startDate,
            endDate,
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
    } else {
      try {
        await createBooking({
          eventDate: startDate,
          eventEndDate: endDate,
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
        });
        navigate('/confirmation');
      } catch (err) {
        alert('Booking failed.');
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
      <h2 className="text-lg font-semibold text-center">
        Booking for: {startDate.toLocaleDateString()} to {endDate.toLocaleDateString()}
      </h2>
      
      <div>
        <label htmlFor="customerName">Name</label>
        <input id="customerName" type="text" value={customerName} onChange={(e) => setCustomerName(e.target.value)} required />
      </div>
      <div>
        <label htmlFor="customerEmail">Email</label>
        <input id="customerEmail" type="email" value={customerEmail} onChange={(e) => setCustomerEmail(e.target.value)} required />
      </div>
      <div>
        <label htmlFor="address">Address</label>
        <input id="address" type="text" value={address} onChange={(e) => setAddress(e.target.value)} required />
      </div>
      <div>
        <label htmlFor="city">City</label>
        <input id="city" type="text" value={city} onChange={(e) => setCity(e.target.value)} required />
      </div>
      <div>
        <label htmlFor="state">State</label>
        <input id="state" type="text" value={state} onChange={(e) => setState(e.target.value)} required />
      </div>
      <div>
        <label htmlFor="zipCode">Zip Code</label>
        <input id="zipCode" type="text" value={zipCode} onChange={(e) => setZipCode(e.target.value)} required />
      </div>

      <div>
        <label htmlFor="sameAsEvent">
          <input
            id="sameAsEvent"
            type="checkbox"
            checked={isBillingSameAsEvent}
            onChange={(e) => setIsBillingSameAsEvent(e.target.checked)}
          />
          Billing address is the same as event address
        </label>
      </div>

      {!isBillingSameAsEvent && (
        <>
          <div>
            <label htmlFor="billingAddress">Billing Address</label>
            <input id="billingAddress" type="text" value={billingAddress} onChange={(e) => setBillingAddress(e.target.value)} required />
          </div>
          <div>
            <label htmlFor="billingCity">Billing City</label>
            <input id="billingCity" type="text" value={billingCity} onChange={(e) => setBillingCity(e.target.value)} required />
          </div>
          <div>
            <label htmlFor="billingState">Billing State</label>
            <input id="billingState" type="text" value={billingState} onChange={(e) => setBillingState(e.target.value)} required />
          </div>
          <div>
            <label htmlFor="billingZipCode">Billing Zip Code</label>
            <input id="billingZipCode" type="text" value={billingZipCode} onChange={(e) => setBillingZipCode(e.target.value)} required />
          </div>
        </>
      )}

      <div className="bg-gray-100 p-4 rounded-md">
        <h3 className="text-lg font-bold mb-2">Price Quote</h3>
        {isFetchingQuote && <p>Calculating price...</p>}
        {quote && !isFetchingQuote && (
            <>
                <p>Rental Fee ({quote.rentalDays} days): ${quote.rentalFee.toFixed(2)}</p>
                <p>Delivery Fee: ${quote.deliveryFee.toFixed(2)}</p>
                <p>Refundable Security Deposit: ${quote.securityDeposit.toFixed(2)}</p>
                <p className="font-bold mt-2">Total Price: ${quote.totalPrice.toFixed(2)}</p>
            </>
        )}
        {!quote && !isFetchingQuote && <p>Please complete the address fields to calculate the price.</p>}
      </div>

      <div>
        <label htmlFor="specialRequests">Special Requests</label>
        <textarea id="specialRequests" value={specialRequests} onChange={(e) => setSpecialRequests(e.target.value)} rows="3" />
      </div>

      <div>
        <label htmlFor="card-element">Credit or debit card</label>
        <div className="p-2 border rounded-md">
          <CardElement id="card-element" options={cardElementOptions} />
        </div>
      </div>

      <button type="submit" disabled={!stripe || !quote || isFetchingQuote} className="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded disabled:bg-gray-400">
        Book Now
      </button>
    </form>
  );
}

export default BookingForm;