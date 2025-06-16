import { useState } from "react";

interface ChallengeProps {
    content: string;
    onSubmit: (isCorrect: boolean) => void;
    isCompleted: boolean;
}

interface VocabularyData {
    question: string;
    options: string[];
}


export default function VocabularyChallenge({ content, onSubmit, isCompleted }: ChallengeProps) {
    const [selectedAnswer, setSelectedAnswer] = useState<string>('');

    const challengeData: VocabularyData = JSON.parse(content);


    return (
        <div className="space-y-6">
            <div className="text-center">
                <h3 className="text-xl font-semibold mb-4">What's the meaning of this word?</h3>
                <div className="text-2xl font-bold text-blue-600 bg-blue-50 p-4 rounded-lg inline-block">
                    {challengeData.question}
                </div>
            </div>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                {challengeData.options.map((option: string, index: number) => (
                    <button key={index} className={`p-4 rounded-lg border-2 transition-all text-left }`} > {option} </button>
                ))}
            </div>
        </div>
    );
};