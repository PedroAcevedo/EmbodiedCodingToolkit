import Task from "../Types/Task";
import { TestCaseResult } from "./testBlocklyCode";

const baseAdress = `http://${process.env.REACT_APP_ADRESS}:8999`;

export default async function getCurrentTaskStatus() {
  var response = await fetch(`${baseAdress}/api/current-task-status`);
  var taskStatus = await response.json();
  return taskStatus as TaskStatus;
}

export async function updateCurrentTaskStatus(
  isCompleted: boolean,
  failedTest: FailedTest | null,
  currentOutput: string,
  testResults: TestCaseResult[],
) {
  const data = new URLSearchParams();
  data.append(
    "data",
    JSON.stringify({
      isCompleted: isCompleted,
      failedTest: failedTest,
      currentOutput: currentOutput,
      testResults: testResults,
    }),
  );

  await fetch(`${baseAdress}/api/mark-current-task-completed`, {
    headers: {
      "Content-Type": "application/x-www-form-urlencoded",
    },
    body: data,
    method: "POST",
  });
}

export async function moveToNextTask() {
  await fetch(`${baseAdress}/api/move-to-next-task`, { method: "POST" });
}

export async function resetTaskStatusInBackend() {
  await fetch(`${baseAdress}/api/reset`, { method: "POST" });
}

export interface TaskStatus {
  task: Task;
  isCompleted: boolean;
  isLastTask: boolean;
}

interface FailedTest {
  inputs: string;
  output: string;
}
