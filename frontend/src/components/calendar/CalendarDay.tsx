"use client";

import { cn } from "@/lib/utils";
import { isToday, isSameDay } from "date-fns";

interface CalendarDayProps {
  date: Date;
  isCurrentMonth: boolean;
  isSelected: boolean;
  exerciseCount: number;
  onClick: (date: Date) => void;
}

export function CalendarDay({
  date,
  isCurrentMonth,
  isSelected,
  exerciseCount,
  onClick,
}: CalendarDayProps) {
  const today = isToday(date);

  return (
    <button
      type="button"
      onClick={() => onClick(date)}
      className={cn(
        "relative flex h-10 w-10 flex-col items-center justify-center rounded-lg text-sm transition-colors",
        "hover:bg-accent hover:text-accent-foreground",
        !isCurrentMonth && "text-muted-foreground/40",
        isCurrentMonth && "text-foreground",
        today && !isSelected && "ring-2 ring-primary font-bold",
        isSelected && "bg-primary text-primary-foreground hover:bg-primary/90"
      )}
    >
      {date.getDate()}
      {exerciseCount > 0 && (
        <span
          className={cn(
            "absolute bottom-0.5 h-1.5 w-1.5 rounded-full",
            isSelected ? "bg-primary-foreground" : "bg-primary"
          )}
        />
      )}
    </button>
  );
}
