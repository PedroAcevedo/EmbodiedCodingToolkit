import Task, { allTasks, TestCases } from "./Tasks";

export default class TaskManager {
  private currentTaskIndex: number = 0;
  private currentTaskCompleted: boolean = false;
  private currentOutput: string = "TEST";
  private failedTest: FailedTest | null = null;
  private testResults: TestCaseResult[] = [];
  private currentCode: string = "";

  private taskOrder: string[] = [
    "caesar_cipher",
    "abundant_number",
    "rle_decompression",
    "star_out",
    "centered_average",
    "not_alone",
    "linear_in",
    "all_task_completed",
  ];

  public setTaskOrder(taskIds: string[]) {
    this.taskOrder = [...taskIds];
    this.currentTaskIndex = 0;
    this.currentTaskCompleted = false;
    this.currentOutput = "";
    this.failedTest = null;
    this.testResults = [];
    this.currentCode = "";
  }

  private get orderedTasks(): Task[] {
    return this.taskOrder.map((taskId) => {
      const task = allTasks.find((task) => task.id === taskId);

      if (!task) {
        throw new Error(`Task not found: ${taskId}`);
      }

      return task;
    });
  }

  public get currentActiveTask() {
    return this.orderedTasks[this.currentTaskIndex];
  }

  public get currentTaskStatus(): TaskStatus {
    return {
      task: this.currentActiveTask,
      isCompleted: this.currentTaskCompleted,
      isLastTask: this.isLastTask,
      currentOutput: this.currentOutput,
      failedTest: this.failedTest,
      testResults: this.testResults,
      currentCode: this.currentCode,
    };
  }

  private get isLastTask() {
    return this.currentTaskIndex == this.orderedTasks.length - 1;
  }

  public moveToNextTask() {
    if (this.isLastTask) return;
    this.currentTaskIndex++;
    this.currentTaskCompleted = false;
  }

  public updateTaskStatus(
    isCompleted: boolean,
    failedTest: FailedTest | null,
    currentOutput: string,
    testResults: TestCaseResult[],
    currentCode: string,
  ) {
    this.currentTaskCompleted = isCompleted;
    this.failedTest = failedTest;
    this.currentOutput = currentOutput;
    this.testResults = testResults;
    this.currentCode = currentCode;
  }
}

interface TaskStatus {
  task: Task;
  isCompleted: boolean;
  isLastTask: boolean;
  failedTest: FailedTest | null;
  currentOutput: string;
  testResults: TestCaseResult[];
  currentCode: string;
}

interface FailedTest {
  inputs: string;
  output: string;
}

interface TestCaseResult {
  inputs: string;
  expectedOutput: string;
  currentOutput: string;
  passed: boolean;
}
