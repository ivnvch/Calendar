"use client";

import { useMemo } from "react";
import {
  startOfMonth,
  endOfMonth,
  startOfWeek,
  endOfWeek,
  eachDayOfInterval,
  addMonths,
  subMonths,
  format,
  isSameMonth,
  isSameDay,
} from "date-fns";
import { ru } from "date-fns/locale";
import { useQuery } from "@tanstack/react-query";
import { ChevronLeft, ChevronRight } from "lucide-react";

import { Button } from "@/components/ui/button";
import { CalendarDay } from "./CalendarDay";
import { getMonthPeriod } from "@/lib/api";
import type { MonthDayPeriod } from "@/lib/types";

const WEEKDAYS = ["Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс"];

interface CalendarGridProps {
  currentMonth: Date;
  selectedDate: Date;
  onMonthChange: (date: Date) => void;
  onDateSelect: (date: Date) => void;
}

export function CalendarGrid({
  currentMonth,
  selectedDate,
  onMonthChange,
  onDateSelect,
}: CalendarGridProps) {
  const year = currentMonth.getFullYear();
  const month = currentMonth.getMonth() + 1;

  const { data: periodData } = useQuery({
    queryKey: ["monthPeriod", year, month],
    queryFn: () => getMonthPeriod(year, month),
  });

  const exerciseMap = useMemo(() => {
    const map = new Map<string, number>();
    periodData?.forEach((p: MonthDayPeriod) => {
      map.set(p.date, p.exerciseCount);
    });
    return map;
  }, [periodData]);

  const days = useMemo(() => {
    const monthStart = startOfMonth(currentMonth);
    const monthEnd = endOfMonth(currentMonth);
    const calStart = startOfWeek(monthStart, { weekStartsOn: 1 });
    const calEnd = endOfWeek(monthEnd, { weekStartsOn: 1 });
    return eachDayOfInterval({ start: calStart, end: calEnd });
  }, [currentMonth]);

  function getExerciseCount(date: Date): number {
    const key = format(date, "yyyy-MM-dd");
    return exerciseMap.get(key) ?? 0;
  }

  return (
    <div className="flex flex-col items-center gap-4">
      <div className="flex w-full items-center justify-between">
        <Button
          variant="ghost"
          size="icon"
          onClick={() => onMonthChange(subMonths(currentMonth, 1))}
        >
          <ChevronLeft className="h-5 w-5" />
        </Button>
        <h2 className="text-lg font-semibold capitalize">
          {format(currentMonth, "LLLL yyyy", { locale: ru })}
        </h2>
        <Button
          variant="ghost"
          size="icon"
          onClick={() => onMonthChange(addMonths(currentMonth, 1))}
        >
          <ChevronRight className="h-5 w-5" />
        </Button>
      </div>

      <div className="grid grid-cols-7 gap-1">
        {WEEKDAYS.map((day) => (
          <div
            key={day}
            className="flex h-10 w-10 items-center justify-center text-xs font-medium text-muted-foreground"
          >
            {day}
          </div>
        ))}

        {days.map((date) => (
          <CalendarDay
            key={date.toISOString()}
            date={date}
            isCurrentMonth={isSameMonth(date, currentMonth)}
            isSelected={isSameDay(date, selectedDate)}
            exerciseCount={getExerciseCount(date)}
            onClick={onDateSelect}
          />
        ))}
      </div>
    </div>
  );
}
