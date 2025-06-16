import { useState } from "react";
import TranslationChallenge from "../Components/ChallengeComponents/TranslationChallenge";
import VocabularyChallenge from "../Components/ChallengeComponents/VocabularyChallenge";
import Headbar from "../Components/Headbar";
import { api } from "../axios/api";


interface DailyChallenge {
    id: number;
    date: string;
    content: string;
    type: ChallengeType;
}

enum ChallengeType {
    Vocabulary = 0,
    Translation = 1,
    GapFill = 2
}

interface ChallengeProps {
    content: string;
    onSubmit: (isCorrect: boolean) => void;
    isCompleted: boolean;
}


export default function DailyChallenge() {
    const [todaysChallenge, setTodaysChallenge] = useState<DailyChallenge | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const [challengeStarted, setChallengeStarted] = useState<boolean>(false);
    const [challengeCompleted, setChallengeCompleted] = useState<boolean>(false);



    const fetchTodaysChallenge = async (): Promise<void> => {
        try {
            setLoading(true);
            const response = await api.get('/api/challenge/today');
            setTodaysChallenge(response.data);
            setError(null);
        } catch (err) {
            setError('Failed to load today\'s challenge');
            console.error('Error fetching challenge:', err);
        } finally {
            setLoading(false);
        }
    };

    const handleStartChallenge = (): void => {
        setChallengeStarted(true);
    };

    async function handleSubmit() {

    }




    function renderChallenge() {
        if (!todaysChallenge) return null;

        const commonProps: ChallengeProps = {
            content: todaysChallenge.content,
            onSubmit: handleSubmit,
            isCompleted: challengeCompleted
        };

        switch (todaysChallenge.type) {
            case ChallengeType.Vocabulary:
                return <VocabularyChallenge {...commonProps} />;
            case ChallengeType.Translation:
                return <TranslationChallenge {...commonProps} />;
        }
    };

    if (loading) {
        return (
            <div className="min-h-screen bg-gray-50">
                <Headbar />
                <div className="pt-16 px-4 md:px-12 max-w-4xl mx-auto">
                    <div className="flex items-center justify-center h-96">
                        <div className="text-center">
                            <p className="text-gray-600">Loading Challenge</p>
                        </div>
                    </div>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="min-h-screen bg-gray-50">
                <Headbar />
                <div className="pt-16 px-4 md:px-12 max-w-4xl mx-auto">
                    <div className="text-center my-8">
                        <h1 className="text-3xl font-bold text-gray-800">Daily Challenge</h1>
                    </div>
                    <div className="bg-red-50 border border-red-200 rounded-lg p-6 text-center">
                        <p className="text-red-700">{error}</p>
                        <button onClick={fetchTodaysChallenge} className="mt-4 bg-red-500 text-white px-4 py-2 rounded-lg hover:bg-red-600 transition-colors">
                            Try again
                        </button>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gray-50">
            <Headbar />
            <div className="pt-16 px-4 md:px-12 max-w-4xl mx-auto">
                <div className="text-center my-8">
                    <h1 className="text-3xl font-bold text-gray-800">Daily Challenge</h1>
                    <p className="text-gray-600 mt-2">
                        {new Date().toLocaleDateString('hu-HU', {
                            weekday: 'long',
                            year: 'numeric',
                            month: 'long',
                            day: 'numeric'
                        })}
                    </p>
                </div>

                {todaysChallenge && !challengeStarted && (
                    <div className="bg-white rounded-xl shadow-md p-6 mb-8">
                        <div className="text-center">
                            <div className="flex items-center justify-center gap-3 mb-4">
                                <span className={`px-3 py-1 rounded-full text-sm font-medium`}> {todaysChallenge.type} </span>
                            </div>
                            <h2 className="text-2xl font-bold text-gray-800 mb-4"> Today's challenge </h2>
                            <button onClick={handleStartChallenge} className="bg-blue-500 text-white px-8 py-3 rounded-lg font-semibold hover:bg-blue-600 transition-colors flex items-center gap-2 mx-auto"> Start Challenge </button>
                        </div>
                    </div>
                )}
                {challengeStarted && (
                    <div className="bg-white rounded-xl shadow-md p-6 mb-8">
                        <div className="mb-6">
                            <div className="flex items-center justify-between mb-4">
                                {challengeCompleted && (
                                    <div className="flex items-center gap-2 text-green-600"> <span className="font-medium">DONE</span> </div>
                                )}
                            </div>
                        </div>
                        {renderChallenge()}
                    </div>
                )}
            </div>
        </div>
    );
};