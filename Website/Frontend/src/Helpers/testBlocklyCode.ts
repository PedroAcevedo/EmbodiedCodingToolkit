import Task from "../Types/Task";

export interface TestCaseResult {
  inputs: string;
  expectedOutput: string;
  currentOutput: string;
  passed: boolean;
}

async function testBlocklyCode(
  code: string,
  task: Task,
): Promise<[boolean, string, string, string, TestCaseResult[]]> {
  return new Promise((resolve, reject) => {
    var allTestsPassed = true;
    var inputs: string[] = [];
    var expectedOutput = "";
    var outputFromCurrentlyBlocklyCode: string = "";

    var testResults: TestCaseResult[] = [];

    var firstFailedInputs: string[] | null = null;
    var firstFailedExpectedOutput = "";
    var firstFailedCurrentOutput = "";

    setTimeout(() => {
      reject();
    }, 500);

    try {
      for (const testCase of task.testCases) {
        outputFromCurrentlyBlocklyCode = "";
        inputs = testCase.inputs;
        expectedOutput = testCase.output;

        eval(
          code +
            `outputFromCurrentlyBlocklyCode = ${task.functionName}(${testCase.inputs.join(",")});`,
        );

        try {
          outputFromCurrentlyBlocklyCode =
            outputFromCurrentlyBlocklyCode.toString();
        } catch (error) {}

        var readableTestInputs = "";
        for (let index = 0; index < inputs.length; index++) {
          const input = inputs[index];
          const variable = task.variables[index];
          readableTestInputs += variable + ": " + input;
          if (index !== inputs.length - 1) {
            readableTestInputs += ", ";
          }
        }

        var passed = outputFromCurrentlyBlocklyCode === testCase.output;

        testResults.push({
          inputs: readableTestInputs,
          expectedOutput: testCase.output,
          currentOutput: outputFromCurrentlyBlocklyCode,
          passed: passed,
        });

        if (!passed) {
          allTestsPassed = false;
          if (firstFailedInputs == null) {
            firstFailedInputs = inputs;
            firstFailedExpectedOutput = expectedOutput;
            firstFailedCurrentOutput = outputFromCurrentlyBlocklyCode;
          }
        }
      }
    } catch (error: any) {
      allTestsPassed = false;
      outputFromCurrentlyBlocklyCode = error.message as string;
    }

    if (firstFailedInputs != null) {
      inputs = firstFailedInputs;
      expectedOutput = firstFailedExpectedOutput;
      outputFromCurrentlyBlocklyCode = firstFailedCurrentOutput;
    }

    var readableInputs = "";
    for (let index = 0; index < inputs.length; index++) {
      const input = inputs[index];
      const variable = task.variables[index];
      readableInputs += variable + ": " + input;
      if (index !== inputs.length - 1) {
        readableInputs += ", ";
      }
    }

    resolve([
      allTestsPassed,
      readableInputs,
      expectedOutput,
      outputFromCurrentlyBlocklyCode,
      testResults,
    ]);

    return;
  });
}

export default testBlocklyCode;
