import { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { api } from '../axios/api';
import Headbar from '../Components/Headbar';

interface WordPair {
    id: number;
    word: string;
    translation: string;
}

interface Wordset {
    id: number;
    name: string;
    description: string;
    difficultyLevel: string;
    topicName: string;
    createdAt: string;
    words: WordPair[];
}

export default function LearnAiWordset() {
    const [wordset, setWordset] = useState<Wordset | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const [currentCardIndex, setCurrentCardIndex] = useState<number>(0);
    const [showTranslation, setShowTranslation] = useState<boolean>(false);
    const [deleteLoading, setDeleteLoading] = useState<boolean>(false);
    const { id } = useParams();


    useEffect(() => {
        async function getWordset() {
            const response = await api.get(`api/ai-wordset/${id}`);
            const data = await response.data;
            setWordset(data);
            setLoading(false);
        }
        getWordset();
    }, [])

    async function handleDelete() {
        const response = await api.delete(`api/ai-wordset/${id}`);
    }


    function handleFlipCard() {
        setShowTranslation(!showTranslation);
    }

    function handleNextCard() {
        if (wordset && currentCardIndex < wordset.words.length - 1) {
            setCurrentCardIndex(currentCardIndex + 1);
            setShowTranslation(false);
        }
    }

    function handlePrevCard() {
        if (currentCardIndex > 0) {
            setCurrentCardIndex(currentCardIndex - 1);
            setShowTranslation(false);
        }
    }

    if (loading) {
        return (
            <>
                <Headbar />
                <div className="max-w-3xl mx-auto pt-16">
                    <div className="flex justify-center items-center min-h-64">
                        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500"></div>
                    </div>
                </div>
            </>
        );
    }

    if (error) {
        return (
            <>
                <Headbar />
                <div className="max-w-3xl mx-auto pt-16">
                    <div className="bg-red-50 border border-red-200 rounded-lg p-6 text-center">
                        <p className="text-red-700">{error}</p>
                        <button onClick={() => window.location.reload()} className="mt-4 px-4 py-2 bg-red-500 text-white rounded hover:bg-red-600" > Try Again </button>
                    </div>
                </div>
            </>
        );
    }

    if (!wordset || !wordset.words || wordset.words.length === 0) {
        return (
            <>
                <Headbar />
                <div className="max-w-3xl mx-auto pt-16">
                    <div className="bg-gray-50 border rounded-lg p-8 text-center">
                        <p className="text-gray-600">This wordset doesn't contain any words yet.</p>
                    </div>
                </div>
            </>
        );
    }

    const currentWord: WordPair = wordset.words[currentCardIndex];

    return (
        <>
            <Headbar />
            <div className="max-w-3xl mx-auto pt-16 px-4">
                <div className="mb-6">
                    <div className="flex items-center justify-between mb-4">
                        <div>
                            <h1 className="text-2xl font-bold mb-1">{wordset.name}</h1>
                        </div>
                        <button onClick={handleDelete} disabled={deleteLoading} className="flex items-center gap-2 px-3 py-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors disabled:opacity-50" title="Delete wordset" > {deleteLoading ? "Deleting..." : "Delete"} </button>
                    </div>
                    <div className="flex items-center gap-4 text-sm text-gray-600">
                        <span>Difficulty: <strong>{wordset.difficultyLevel}</strong></span>
                        <span>Topic: <strong>{wordset.topicName}</strong></span>
                        <span>{wordset.words.length} words</span>
                    </div>
                </div>
                <div className="mb-6">
                    <div className="flex justify-between items-center mb-4">
                        <span className="text-sm text-gray-600">
                            Card: {currentCardIndex + 1}/{wordset.words.length}
                        </span>
                    </div>
                    <div className="bg-white border rounded-xl p-8 shadow-lg cursor-pointer min-h-64 flex flex-col justify-center items-center hover:shadow-xl transition-shadow" onClick={handleFlipCard} >
                        <div className="text-center">
                            {!showTranslation ? (
                                <>
                                    <div className="text-3xl font-medium text-gray-800 mb-4"> {currentWord.word} </div>

                                </>
                            ) : (
                                <>
                                    <div className="text-3xl font-medium text-blue-600 mb-4"> {currentWord.translation} </div>
                                </>
                            )}
                        </div>
                    </div>
                </div>
                <div className="flex justify-between mb-8">
                    <button onClick={handlePrevCard} disabled={currentCardIndex === 0} className={`flex items-center gap-2 px-6 py-3 rounded-lg font-medium transition-all ${currentCardIndex === 0 ? "bg-gray-100 text-gray-400 cursor-not-allowed" : "bg-blue-500 text-white hover:bg-blue-600 shadow-md hover:shadow-lg"}`}  > Previous </button>
                    <button onClick={handleNextCard} disabled={currentCardIndex === wordset.words.length - 1} className={`flex items-center gap-2 px-6 py-3 rounded-lg font-medium transition-all ${currentCardIndex === wordset.words.length - 1 ? "bg-gray-100 text-gray-400 cursor-not-allowed" : "bg-blue-500 text-white hover:bg-blue-600 shadow-md hover:shadow-lg"}`}  > Next  </button>
                </div>
            </div>
        </>
    );
};
