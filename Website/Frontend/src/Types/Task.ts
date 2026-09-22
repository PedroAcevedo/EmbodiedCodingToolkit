export default interface Task {
    id: string,
    title: string,
    description: string,
    example: string,
    functionName: string,
    variables: string[],
    testCases: TestCases[]
}

interface TestCases
{
    inputs: string[],
    output: string
}