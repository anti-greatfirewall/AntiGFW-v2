﻿using System;
using Microsoft.Win32.TaskScheduler;

namespace AntiGFW {
    internal static class TaskScheduler {
        private static readonly TaskService TaskService = new TaskService();

        internal static void DeleteTask() {
            TaskService.RootFolder.DeleteTask(Utils.ProgramName, false);
        }

        internal static void CreateTask() {
            DateTime now = DateTime.Now;
            DateTime dateTime = new DateTime(now.Year, now.Month, now.Day - 1).AddSeconds(10);
            ExecAction execAction = new ExecAction(Utils.ExePath, null, Utils.ExeDirectory);
            TaskDefinition taskDefinition = TaskService.NewTask();
            taskDefinition.RegistrationInfo.Description = Utils.ProgramName;
            taskDefinition.Principal.LogonType = TaskLogonType.InteractiveToken;
            DailyTrigger dailyTrigger = new DailyTrigger {
                StartBoundary = dateTime,
                Repetition = {
                    Duration = TimeSpan.Zero,
                    Interval = TimeSpan.FromHours(1)
                }
            };
            taskDefinition.Triggers.Add(dailyTrigger);
            taskDefinition.Actions.Add(execAction);
            TaskService.RootFolder.DeleteTask(Utils.ProgramName, false);
            TaskService.RootFolder.RegisterTaskDefinition(Utils.ProgramName, taskDefinition);
        }

        internal static bool FindTask() {
            return TaskService.RootFolder.GetTasks().Exists(Utils.ProgramName);
        }
    }
}
