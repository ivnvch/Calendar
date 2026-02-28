"use client";

import { useState } from "react";
import { startOfMonth } from "date-fns";

import { CalendarGrid } from "@/components/calendar/CalendarGrid";
import { DayDashboard } from "@/components/dashboard/DayDashboard";

export default function Home() {
  const [selectedDate, setSelectedDate] = useState(new Date());
  const [currentMonth, setCurrentMonth] = useState(startOfMonth(new Date()));

  function handleDateSelect(date: Date) {
    setSelectedDate(date);
    setCurrentMonth(startOfMonth(date));
  }

  return (
    <div className="min-h-screen bg-background">
      <header className="border-b">
        <div className="mx-auto max-w-6xl px-4 py-4">
          <h1 className="text-2xl font-bold tracking-tight">
            Спортивный календарь
          </h1>
        </div>
      </header>

      <main className="mx-auto max-w-6xl px-4 py-6">
        <div className="grid grid-cols-1 gap-6 lg:grid-cols-[auto_1fr]">
          <div className="flex justify-center lg:justify-start">
            <CalendarGrid
              currentMonth={currentMonth}
              selectedDate={selectedDate}
              onMonthChange={setCurrentMonth}
              onDateSelect={handleDateSelect}
            />
          </div>

          <div className="min-w-0">
            <DayDashboard selectedDate={selectedDate} />
          </div>
        </div>
      </main>
    </div>
  );
}
