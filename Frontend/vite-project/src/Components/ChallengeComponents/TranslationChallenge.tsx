import { useState } from "react";

interface ChallengeProps {
    content: string;
    onSubmit: (isCorrect: boolean) => void;
    isCompleted: boolean;
}

interface TranslationData {
    questions: string;
}

export default function TranslationChallenge({ content, onSubmit, isCompleted }: ChallengeProps) {
    const [userAnswer, setUserAnswer] = useState<string>('');
    const [showResult, setShowResult] = useState<boolean>(false);

    const challengeData: TranslationData = JSON.parse(content);

    return (
        <div className="space-y-6">
            <div className="text-center">
                <h3 className="text-xl font-semibold mb-4">Translate this sentence!</h3>
                <div className="text-lg bg-gray-50 p-4 rounded-lg">
                    <div className="mt-2 text-xl font-semibold text-gray-800">
                        "{challengeData.questions}"
                    </div>
                </div>
            </div>
            <div>
                <textarea
                    value={userAnswer}
                    onChange={(e: React.ChangeEvent<HTMLTextAreaElement>) => setUserAnswer(e.target.value)}
                    disabled={showResult || isCompleted}
                    className="w-full p-4 border-2 border-gray-300 rounded-lg focus:border-blue-500 focus:outline-none resize-none h-24"
                />
            </div>
            {!showResult && !isCompleted && (
                <button disabled={!userAnswer.trim()} className="w-full bg-blue-500 text-white py-3 rounded-lg font-semibold hover:bg-blue-600 disabled:bg-gray-300 disabled:cursor-not-allowed transition-colors"> Submit </button>
            )}
        </div>
    );
};