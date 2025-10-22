import React from 'react';

function FaqPage() {
  const faqs = [
    {
      question: "What is the cancellation policy?",
      answer: "You can cancel up to 7 days before your event for a full refund. Cancellations within 7 days are non-refundable."
    },
    {
      question: "What happens in case of bad weather?",
      answer: "We can set up the tent in light to moderate rain. If there are high winds or severe weather, we will need to reschedule. Your payment will be applied to a future rental date."
    },
    {
      question: "What is your service area?",
      answer: "We serve a 30-mile radius from our location. A delivery fee may apply for locations over 15 miles away."
    },
    {
      question: "What is required for setup?",
      answer: "We need a relatively flat, open area of at least 20x30 feet. The area must be clear of any obstacles and overhead power lines."
    },
    {
      question: "What are the payment options?",
      answer: "We accept all major credit cards. Payment is due in full at the time of booking."
    }
  ];

  return (
    <div className="min-h-screen bg-white dark:bg-gray-900 text-gray-900 dark:text-white flex flex-col items-center p-4 sm:p-8">
      <div className="w-full max-w-4xl bg-gray-100 dark:bg-gray-800 p-6 sm:p-8 rounded-lg shadow-lg">
        <h1 className="text-3xl sm:text-4xl font-bold text-center text-blue-600 dark:text-blue-400 mb-6 sm:mb-8">Frequently Asked Questions</h1>
        <div className="space-y-4">
          {faqs.map((faq, index) => (
            <div key={index} className="bg-gray-50 dark:bg-gray-700 p-4 rounded-lg border border-gray-200 dark:border-gray-600">
              <h2 className="text-xl font-semibold text-blue-600 dark:text-blue-300">{faq.question}</h2>
              <p className="mt-2 text-gray-700 dark:text-gray-300">{faq.answer}</p>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

export default FaqPage;
