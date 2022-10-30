export interface ActiveErrors{
    errorData?:       [];
    error:            string;
    notFound:         boolean;
}

export interface ErrorData {
    errorId:          number;
    projectName:      string;
    errorDescription: string;
    dateStarted:      string;
}