import Task, { allTasks, TestCases } from "./Tasks";

export default class TaskManager {
  private currentTaskIndex: number = 0;
  private currentTaskCompleted: boolean = false;
  private currentOutput: string = "TEST";
  private failedTest: FailedTest | null = null;
  private testResults: TestCaseResult[] = [];

  public get currentActiveTask() {
    return allTasks[this.currentTaskIndex];
  }

  public get currentTaskStatus(): TaskStatus {
    return {
      task: this.currentActiveTask,
      isCompleted: this.currentTaskCompleted,
      isLastTask: this.isLastTask,
      currentOutput: this.currentOutput,
      failedTest: this.failedTest,
      testResults: this.testResults,
    };
  }

  private get isLastTask() {
    return this.currentTaskIndex == allTasks.length - 1;
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
  ) {
    this.currentTaskCompleted = isCompleted;
    this.failedTest = failedTest;
    this.currentOutput = currentOutput;
    this.testResults = testResults;
  }
}

interface TaskStatus {
  task: Task;
  isCompleted: boolean;
  isLastTask: boolean;
  failedTest: FailedTest | null;
  currentOutput: string;
  testResults: TestCaseResult[];
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
