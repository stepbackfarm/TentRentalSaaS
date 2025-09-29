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

  return (
    <form onSubmit={handleSubmit}>
      <h2>Booking Details for {selectedDate ? selectedDate.toDateString() : 'N/A'}</h2>
      <div>
        <label htmlFor="customerName">Name:</label>
        <input
          type="text"
          id="customerName"
          value={customerName}
          onChange={(e) => setCustomerName(e.target.value)}
          required
        />
      </div>
      <div>
        <label htmlFor="customerEmail">Email:</label>
        <input
          type="email"
          id="customerEmail"
          value={customerEmail}
          onChange={(e) => setCustomerEmail(e.target.value)}
          required
        />
      </div>
      <div>
        <label htmlFor="card-element">Credit or debit card</label>
        <CardElement id="card-element" />
      </div>
      <button type="submit" disabled={!stripe || !selectedDate}>
        Book Now
      </button>
    </form>
  );
}

export default BookingForm;
