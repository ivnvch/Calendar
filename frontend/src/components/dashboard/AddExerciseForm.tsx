"use client";

import { useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Plus, Trash2 } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
  DialogFooter,
} from "@/components/ui/dialog";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import type { ActivityType, CreateExerciseDto } from "@/lib/types";
import { createWorkoutDay, getWorkoutDay } from "@/lib/api";
import { ACTIVITY_META, ACTIVITY_TYPES, cn } from "@/lib/utils";

interface AddExerciseFormProps {
  date: string;
}

export function AddExerciseForm({ date }: AddExerciseFormProps) {
  const [open, setOpen] = useState(false);
  const [exercises, setExercises] = useState<CreateExerciseDto[]>([]);
  const [activityType, setActivityType] = useState<ActivityType | "">("");
  const [targetValue, setTargetValue] = useState("");
  const [showPastDateError, setShowPastDateError] = useState(false);
  const queryClient = useQueryClient();

  const isPastDate = date < new Date().toISOString().split("T")[0];

  // Проверяем, есть ли уже тренировка на эту дату (только когда диалог открыт)
  const { data: workoutDay, error: workoutDayError } = useQuery({
    queryKey: ["workoutDay", date],
    queryFn: () => getWorkoutDay(date),
    retry: false,
    staleTime: Infinity,
    enabled: open, // Запрашиваем только при открытом диалоге
  });

  // 404 означает, что тренировки нет - это нормальная ситуация
  const hasWorkout = workoutDay !== undefined && workoutDay !== null && workoutDayError === undefined;

  const mutation = useMutation({
    mutationFn: (list: CreateExerciseDto[]) => createWorkoutDay(date, list),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["workoutDay", date] });
      queryClient.invalidateQueries({ queryKey: ["monthPeriod"] });
      resetForm();
      setOpen(false);
    },
    retry: false,
  });

  function getErrorMessage(message: string): string {
    if (message.includes("past")) {
      return "Нельзя создать тренировку на прошедшую дату";
    }
    if (message.includes("already exists") || message.includes("conflict")) {
      return "На эту дату уже есть тренировка";
    }
    return `Ошибка: ${message}`;
  }

  function resetForm() {
    setExercises([]);
    setActivityType("");
    setTargetValue("");
  }

  function handleAdd() {
    if (!activityType || !targetValue) return;
    const value = parseFloat(targetValue);
    if (isNaN(value) || value <= 0) return;

    setExercises((prev) => [
      ...prev,
      { activityType: activityType as ActivityType, targetValue: value },
    ]);
    setActivityType("");
    setTargetValue("");
  }

  function handleRemove(index: number) {
    setExercises((prev) => prev.filter((_, i) => i !== index));
  }

  function handleSubmit() {
    if (exercises.length === 0) return;
    mutation.mutate(exercises);
  }

  const selectedMeta = activityType ? ACTIVITY_META[activityType] : null;

  return (
    <Dialog
      open={open}
      onOpenChange={(v) => {
        setOpen(v);
        if (!v) resetForm();
      }}
    >
      <DialogTrigger asChild>
        <Button
          size="sm"
          disabled={isPastDate || hasWorkout}
          onClick={(e) => {
            if (isPastDate) {
              e.preventDefault();
              setShowPastDateError(true);
              setTimeout(() => setShowPastDateError(false), 3000);
            }
          }}
        >
          <Plus className="mr-1 h-4 w-4" />
          Добавить
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-lg">
        <DialogHeader>
          <DialogTitle>Новая тренировка</DialogTitle>
        </DialogHeader>

        {showPastDateError && (
          <div className="rounded-md bg-destructive/10 p-3 text-sm text-destructive">
            Нельзя создать тренировку на прошедшую дату
          </div>
        )}

        {hasWorkout && (
          <div className="rounded-md bg-destructive/10 p-3 text-sm text-destructive">
            На эту дату уже есть тренировка
          </div>
        )}

        <div className="space-y-4 py-4">
          {exercises.length > 0 && (
            <div className="space-y-2">
              <label className="text-sm font-medium">
                Добавленные упражнения
              </label>
              <div className="space-y-2">
                {exercises.map((ex, i) => {
                  const meta = ACTIVITY_META[ex.activityType];
                  return (
                    <div
                      key={i}
                      className="flex items-center justify-between rounded-lg border px-3 py-2"
                    >
                      <div className="flex items-center gap-2">
                        <span
                          className={cn(
                            "inline-flex h-7 w-7 items-center justify-center rounded text-xs font-bold",
                            meta.bgColor,
                            meta.color
                          )}
                        >
                          {meta.label.slice(0, 2)}
                        </span>
                        <span className="text-sm font-medium">
                          {meta.label}
                        </span>
                        <span className="text-sm text-muted-foreground">
                          {ex.targetValue} {meta.unit}
                        </span>
                      </div>
                      <Button
                        variant="ghost"
                        size="icon"
                        className="h-7 w-7 text-muted-foreground hover:text-destructive"
                        onClick={() => handleRemove(i)}
                      >
                        <Trash2 className="h-4 w-4" />
                      </Button>
                    </div>
                  );
                })}
              </div>
            </div>
          )}

          <div className="space-y-2">
            <label className="text-sm font-medium">Тип активности</label>
            <Select
              value={activityType}
              onValueChange={(v) => setActivityType(v as ActivityType)}
            >
              <SelectTrigger>
                <SelectValue placeholder="Выберите тип" />
              </SelectTrigger>
              <SelectContent>
                {ACTIVITY_TYPES.map((type) => (
                  <SelectItem key={type} value={type}>
                    {ACTIVITY_META[type].label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium">
              Целевое значение
              {selectedMeta && (
                <span className="ml-1 text-muted-foreground">
                  ({selectedMeta.unit})
                </span>
              )}
            </label>
            <Input
              type="number"
              min="0.1"
              step="0.1"
              placeholder={
                selectedMeta
                  ? `Например: 5 ${selectedMeta.unit}`
                  : "Введите значение"
              }
              value={targetValue}
              onChange={(e) => setTargetValue(e.target.value)}
            />
          </div>

          <Button
            variant="secondary"
            className="w-full"
            onClick={handleAdd}
            disabled={!activityType || !targetValue}
          >
            <Plus className="mr-1 h-4 w-4" />
            Добавить в список
          </Button>

          {mutation.isError && (
            <p className="text-sm text-destructive">
              {getErrorMessage(mutation.error.message)}
            </p>
          )}
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => setOpen(false)}>
            Отмена
          </Button>
          <Button
            onClick={handleSubmit}
            disabled={exercises.length === 0 || mutation.isPending}
          >
            {mutation.isPending
              ? "Сохранение..."
              : `Сохранить (${exercises.length})`}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
