import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { api } from '../axios/api';

interface WordPair {
    id: number;
    first: string;
    second: string;
}

interface Wordset {
    id: number;
    name: string;
    firstLanguage: string;
    secondLanguage: string;
    wordPairs: WordPair[];
    createdAt: string;
    lastPracticed?: string;
}

interface RouteParams {
    wordsetId: string;
}

export default function LearnWordset() {
    const [wordset, setWordset] = useState<Wordset | null>(null);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const [currentCardIndex, setCurrentCardIndex] = useState<number>(0);
    const [isFlipped, setIsFlipped] = useState<boolean>(false);
    const [showProgress, setShowProgress] = useState<boolean>(false);
    const [knownWords, setKnownWords] = useState<Set<number>>(new Set());
    const { wordsetId } = useParams();
    const navigate = useNavigate();

    useEffect(() => {
        console.log(wordset);
    }, [wordset])

    useEffect(() => {
        const fetchWordset = async (): Promise<void> => {
            try {
                setIsLoading(true);
                const response = await api.get(`/api/wordset/${wordsetId}`);

                if (response.status != 200) {
                    throw new Error('Failed to fetch wordset');
                }

                const data: Wordset = await response.data;
                setWordset(data);
                console.log(data);

            } catch (err) {
                setError(err instanceof Error ? err.message : 'An unknown error occurred');
            } finally {
                setIsLoading(false);
            }
        };

        fetchWordset();
    }, [wordsetId]);

    const handleBackToWordsets = (): void => {
        navigate(-1);
    };

    const handleFlipCard = (): void => {
        setIsFlipped(!isFlipped);
    };

    const handleNextCard = (): void => {
        if (!wordset || !wordset.wordPairs.length) return;

        setIsFlipped(false);
        if (currentCardIndex < wordset.wordPairs.length - 1) {
            setCurrentCardIndex(currentCardIndex + 1);
        } else {
            // When we reach the end, show progress screen
            setShowProgress(true);
        }
    };

    const handlePreviousCard = (): void => {
        if (!wordset || currentCardIndex <= 0) return;

        setIsFlipped(false);
        setCurrentCardIndex(currentCardIndex - 1);
    };

    const handleMarkAsKnown = (): void => {
        if (!wordset) return;

        const currentId = wordset.wordPairs[currentCardIndex].id;
        const newKnownWords = new Set(knownWords);

        if (newKnownWords.has(currentId)) {
            newKnownWords.delete(currentId);
        } else {
            newKnownWords.add(currentId);
        }

        setKnownWords(newKnownWords);
        handleNextCard();
    };

    const handleRestartLearning = (): void => {
        setCurrentCardIndex(0);
        setIsFlipped(false);
        setShowProgress(false);
    };

    const shuffleCards = (): void => {
        if (!wordset) return;

        const shuffled = [...wordset.wordPairs].sort(() => Math.random() - 0.5);
        setWordset({ ...wordset, wordPairs: shuffled });
        setCurrentCardIndex(0);
        setIsFlipped(false);
    };

    if (isLoading) {
        return (
            <div className="min-h-screen bg-gray-50 flex items-center justify-center">
                <div className="text-center">
                    <div className="text-gray-600">Loading wordset...</div>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="min-h-screen bg-gray-50 pt-16 px-4 md:px-12 max-w-7xl mx-auto">
                <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
                    {error}
                </div>
                <div className="mt-4">
                    <button
                        onClick={handleBackToWordsets}
                        className="text-blue-500 hover:text-blue-700 font-medium"
                    >
                        &larr; Back to My Wordsets
                    </button>
                </div>
            </div>
        );
    }

    if (!wordset || !wordset.wordPairs.length) {
        return (
            <div className="min-h-screen bg-gray-50 pt-16 px-4 md:px-12 max-w-7xl mx-auto">
                <div className="bg-white rounded-xl shadow-md p-10 mb-8 text-center">
                    <h1 className="text-2xl font-bold text-gray-800 mb-4">No Words Found</h1>
                    <p className="text-gray-600 mb-6">This wordset doesn't contain any words yet.</p>
                    <button
                        onClick={handleBackToWordsets}
                        className="bg-blue-500 hover:bg-blue-600 text-white px-6 py-2 rounded-md"
                    >
                        Back to My Wordsets
                    </button>
                </div>
            </div>
        );
    }

    if (showProgress) {
        return (
            <div className="min-h-screen bg-gray-50 pt-16 px-4 md:px-12 max-w-7xl mx-auto">
                <div className="bg-white rounded-xl shadow-md p-8 mb-8 max-w-2xl mx-auto">
                    <div className="text-center mb-8">
                        <h1 className="text-3xl font-bold text-gray-800 mb-2">Session Complete!</h1>
                        <p className="text-gray-600">
                            You studied {wordset.wordPairs.length} words from "{wordset.name}"
                        </p>
                    </div>

                    <div className="mb-8">
                        <div className="bg-blue-50 p-6 rounded-lg text-center">
                            <div className="text-4xl font-bold text-blue-600 mb-2">
                                {knownWords.size} / {wordset.wordPairs.length}
                            </div>
                            <div className="text-blue-700">Words marked as known</div>
                        </div>
                    </div>

                    <div className="flex justify-between">
                        <button
                            onClick={handleBackToWordsets}
                            className="px-6 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-100"
                        >
                            Back to Wordsets
                        </button>
                        <button
                            onClick={handleRestartLearning}
                            className="px-6 py-2 bg-blue-500 hover:bg-blue-600 text-white rounded-md"
                        >
                            Study Again
                        </button>
                    </div>
                </div>
            </div>
        );
    }

    const currentWord = wordset.wordPairs[currentCardIndex];
    const isCurrentWordKnown = knownWords.has(currentWord.id);

    return (
        <div className="min-h-screen bg-gray-50">
            <div className="pt-16 px-4 md:px-12 max-w-7xl mx-auto">
                <div className="mb-6 flex justify-between items-center">
                    <button
                        onClick={handleBackToWordsets}
                        className="text-blue-500 hover:text-blue-700 font-medium flex items-center"
                    >
                        <span className="mr-1">&larr;</span> Back to Wordsets
                    </button>
                    <div className="text-gray-600">
                        {currentCardIndex + 1} / {wordset.wordPairs.length}
                    </div>
                </div>

                <div className="text-center mb-8">
                    <h1 className="text-3xl font-bold text-gray-800">{wordset.name}</h1>
                    <div className="text-gray-600 mt-2">
                        {wordset.firstLanguage} to {wordset.secondLanguage}
                    </div>
                </div>

                <div className="max-w-2xl mx-auto mb-8">
                    <div
                        className={`bg-white rounded-xl shadow-lg p-8 min-h-64 flex flex-col justify-center items-center cursor-pointer transition-all duration-300 transform ${isFlipped ? 'bg-blue-50' : ''
                            }`}
                        onClick={handleFlipCard}
                        style={{ perspective: '1000px', height: '300px' }}
                    >
                        <div className="text-sm text-gray-500 mb-6">
                            {isFlipped ? wordset.secondLanguage : wordset.firstLanguage}
                        </div>
                        <div className="text-4xl font-bold text-center">
                            {isFlipped ? currentWord.second : currentWord.first}
                        </div>
                        <div className="mt-6 text-sm text-blue-500">Click to flip</div>
                    </div>
                </div>

                <div className="flex justify-center gap-4 mb-8">
                    <button
                        onClick={handlePreviousCard}
                        disabled={currentCardIndex === 0}
                        className={`px-6 py-2 rounded-md flex items-center ${currentCardIndex === 0
                            ? 'bg-gray-200 text-gray-400 cursor-not-allowed'
                            : 'bg-gray-200 hover:bg-gray-300 text-gray-700'
                            }`}
                    >
                        Previous
                    </button>
                    <button
                        onClick={shuffleCards}
                        className="px-6 py-2 bg-purple-500 hover:bg-purple-600 text-white rounded-md"
                    >
                        Shuffle
                    </button>
                    <button
                        onClick={handleMarkAsKnown}
                        className={`px-6 py-2 rounded-md ${isCurrentWordKnown
                            ? 'bg-yellow-500 hover:bg-yellow-600 text-white'
                            : 'bg-green-500 hover:bg-green-600 text-white'
                            }`}
                    >
                        {isCurrentWordKnown ? 'Mark as Unknown' : 'Mark as Known'}
                    </button>
                    <button
                        onClick={handleNextCard}
                        className="px-6 py-2 bg-blue-500 hover:bg-blue-600 text-white rounded-md"
                    >
                        Next
                    </button>
                </div>

                <div className="bg-white rounded-xl shadow-md p-6 max-w-2xl mx-auto">
                    <div className="flex justify-between items-center mb-4">
                        <h2 className="text-xl font-semibold text-gray-800">Progress</h2>
                        <div className="text-sm text-gray-600">
                            {knownWords.size} of {wordset.wordPairs.length} words known
                        </div>
                    </div>
                    <div className="relative h-2 bg-gray-200 rounded-full overflow-hidden">
                        <div
                            className="absolute top-0 left-0 h-full bg-green-500"
                            style={{
                                width: `${(knownWords.size / wordset.wordPairs.length) * 100}%`,
                            }}
                        ></div>
                    </div>
                </div>
            </div>
        </div>
    );
}