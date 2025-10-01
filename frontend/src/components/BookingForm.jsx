import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { CardElement, useStripe, useElements } from '@stripe/react-stripe-js';
import { createBooking } from '../services/api';

function BookingForm({ selectedDate }) {
  const [customerName, setCustomerName] = useState('');
  const [customerEmail, setCustomerEmail] = useState('');
  const stripe = useStripe();
  const elements = useElements();
  const navigate = useNavigate();

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
      console.log('[PaymentMethod]', paymentMethod);
      try {
        await createBooking({
          eventDate: selectedDate,
          customerName,
          customerEmail,
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
        Booking for: {selectedDate ? selectedDate.toLocaleDateString() : 'N/A'}
      </h2>
      <div>
        <label htmlFor="customerName" className="block text-sm font-bold text-gray-300 mb-1">Name</label>
        <input
          type="text"
          id="customerName"
          value={customerName}
          onChange={(e) => setCustomerName(e.target.value)}
          required
          className="p-2 rounded-md border border-gray-300 w-full"
        />
      </div>
      <div>
        <label htmlFor="customerEmail" className="block text-sm font-bold text-gray-300 mb-1">Email</label>
        <input
          type="email"
          id="customerEmail"
          value={customerEmail}
          onChange={(e) => setCustomerEmail(e.target.value)}
          required
          className="p-2 rounded-md border border-gray-300 w-full"
        />
      </div>
      <div>
        <label htmlFor="card-element" className="block text-sm font-bold text-gray-300 mb-1">Credit or debit card</label>
        <div className="p-2 rounded-md border border-gray-300">
          <CardElement id="card-element" options={cardElementOptions} />
        </div>
      </div>
      <button
        type="submit"
        disabled={!stripe || !selectedDate}
        className="w-full button-primary font-bold py-2 px-4 rounded-md transition-colors duration-300 disabled:bg-gray-500 disabled:cursor-not-allowed"
      >
        Book Now
      </button>
    </form>
  );
}

export default BookingForm;
