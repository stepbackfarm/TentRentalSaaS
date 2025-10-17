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
  const [tentType, setTentType] = useState('Standard');
  const [numberOfTents, setNumberOfTents] = useState(1);
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
          setQuote(null); // Clear quote on error
        }
        setIsFetchingQuote(false);
      }
    };

    const debounce = setTimeout(() => {
        fetchQuote();
    }, 500); // Debounce to avoid excessive API calls while typing

    return () => clearTimeout(debounce);
  }, [startDate, endDate, address, city, state, zipCode]);

  const handleSubmit = async (event) => {
    event.preventDefault();
    if (!stripe || !elements) {
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
          tentType,
          numberOfTents,
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
        color: '#ffffff',
        fontFamily: '"Helvetica Neue", Helvetica, sans-serif',
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
    <form onSubmit={handleSubmit} className="space-y-6">
      <h2 className="text-lg font-semibold text-center text-white">
        Booking for: {startDate.toLocaleDateString()} to {endDate.toLocaleDateString()}
      </h2>
      
      {/* Customer and Address Fields... */}
      {/* ... (All input fields remain the same) ... */}

      <div className="text-white bg-gray-700 p-4 rounded-lg">
        <h3 className="text-lg font-bold mb-2">Price Quote</h3>
        {isFetchingQuote && <p>Calculating price...</p>}
        {quote && !isFetchingQuote && (
            <>
                <p>Rental Fee ({quote.rentalDays} days): ${quote.rentalFee.toFixed(2)}</p>
                <p>Delivery Fee: ${quote.deliveryFee.toFixed(2)}</p>
                <p>Refundable Security Deposit: ${quote.securityDeposit.toFixed(2)}</p>
                <p className="font-bold text-blue-400 mt-2">Total Price: ${quote.totalPrice.toFixed(2)}</p>
            </>
        )}
        {!quote && !isFetchingQuote && <p>Please complete the address fields to calculate the price.</p>}
      </div>

      {/* Tent Type, Number of Tents, Special Requests... */}
      {/* ... (These input fields remain the same) ... */}

      <div>
        <label htmlFor="card-element" className="block text-sm font-bold text-gray-300 mb-1">Credit or debit card</label>
        <div className="p-2 rounded-md border border-gray-300 min-h-[42px] flex items-center bg-gray-700">
          <CardElement id="card-element" options={cardElementOptions} className="w-full" />
        </div>
      </div>
      <button
        type="submit"
        disabled={!stripe || !quote || isFetchingQuote}
        className="w-full button-primary font-bold py-2 px-4 rounded-md transition-colors duration-300 disabled:bg-gray-500 disabled:cursor-not-allowed"
      >
        Book Now
      </button>
    </form>
  );
}

export default BookingForm;
