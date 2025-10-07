import React from 'react';
import { Link } from 'react-router-dom';

function ConfirmationPage() {
  const eventDetails = {
    title: "Your Tent Rental",
    description: "Your tent rental is confirmed.",
    location: "Your Event Address",
    startTime: new Date(new Date().getTime() + 7 * 24 * 60 * 60 * 1000).toISOString(),
    endTime: new Date(new Date().getTime() + (7 * 24 * 60 * 60 * 1000) + (2 * 60 * 60 * 1000)).toISOString()
  };

  const createIcsFile = () => {
    const formatISOString = (isoString) => isoString.replace(/[-:.]/g, '');

    const icsContent = `BEGIN:VCALENDAR
VERSION:2.0
PRODID:-//Your Company//Your Product//EN
BEGIN:VEVENT
UID:${new Date().getTime()}@yourcompany.com
DTSTAMP:${formatISOString(new Date().toISOString())}
DTSTART:${formatISOString(eventDetails.startTime)}
DTEND:${formatISOString(eventDetails.endTime)}
SUMMARY:${eventDetails.title}
DESCRIPTION:${eventDetails.description}
LOCATION:${eventDetails.location}
END:VEVENT
END:VCALENDAR`;

    const blob = new Blob([icsContent], { type: 'text/calendar;charset=utf-8' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = 'event.ics';
    link.click();
  };

  return (
    <div className="min-h-screen bg-gray-900 text-white flex flex-col items-center justify-center p-4">
      <div className="bg-gray-800 p-8 rounded-lg shadow-lg text-center max-w-md w-full">
        <h1 className="text-3xl font-bold text-green-400 mb-4">Booking Confirmed!</h1>
        <p className="text-gray-300 mb-6">Your tent is booked. We've sent a confirmation to your email.</p>
        
        <div className="text-left my-6 p-4 border-t border-b border-gray-700">
          <h2 className="text-xl font-bold text-blue-400 mb-3">What to Expect Next</h2>
          <ul className="list-disc list-inside text-gray-300 space-y-2">
            <li>You will receive a confirmation email shortly.</li>
            <li>We will contact you 2-3 days before your event to confirm setup details.</li>
            <li>Our team will arrive on the day of your event to set up the tent.</li>
            <li>After your event, we will return to take down the tent.</li>
          </ul>
        </div>

        <div className="text-center my-6">
          <h2 className="text-xl font-bold text-blue-400 mb-3">Contact Us</h2>
          <p className="text-gray-300">Have questions? Email us at <a href="mailto:contact@tentrentalsaas.com" className="text-blue-400 hover:underline">contact@tentrentalsaas.com</a></p>
        </div>

        <div className="my-6">
          <button 
            onClick={createIcsFile}
            className="bg-green-600 hover:bg-green-700 text-white font-bold py-2 px-4 rounded-md transition-colors duration-300 mr-2"
          >
            Add to Calendar
          </button>
          <Link 
            to="/"
            className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-md transition-colors duration-300"
          >
            Book Another Event
          </Link>
        </div>

        <div className="text-center mt-6">
          <h2 className="text-xl font-bold text-blue-400 mb-3">Share Your Event!</h2>
          <div className="flex justify-center space-x-4">
            <a href="https://www.facebook.com/sharer/sharer.php?u=https://tentrentalsaas.com" target="_blank" rel="noopener noreferrer" className="text-blue-500 hover:text-blue-400">Facebook</a>
            <a href="https://twitter.com/intent/tweet?url=https://tentrentalsaas.com&text=We just booked a tent for our event!" target="_blank" rel="noopener noreferrer" className="text-blue-400 hover:text-blue-300">Twitter</a>
          </div>
        </div>

      </div>
    </div>
  );
}

export default ConfirmationPage;
